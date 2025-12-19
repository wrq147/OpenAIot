using ChannelUtility;
using ChannelUtility.Message;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ModbusChannel
{
    public class GpioTask
    {
        private IServiceProvider _provider;
        private GpioController _controller;
        private ClientBusProxy _eventBus;
        private string _dtuId;
        private HashSet<int> _openPins = new HashSet<int>();
        private ConcurrentQueue<RawDataMessage> _msgQueue = new ConcurrentQueue<RawDataMessage>();
        private Task _mainTask;
        private bool _isAborted;
        ManualResetEvent _manualResetEvent = new ManualResetEvent(false);
        public GpioTask(string dtuId, IServiceProvider provider)
        {
            _dtuId = dtuId;
            _provider = provider;
        }
        public async Task Start(CancellationToken stoppingToken)
        {
            _controller = new GpioController();
            _eventBus = _provider.GetService<ClientBusProxy>();
            _eventBus.OnSubProductMessage += SuProductHandler;
            _mainTask = Task.Run(async () =>
            {
                while (!_isAborted)
                {
                    try
                    {
                        await Exe(stoppingToken);
                    }
                    catch (TaskCanceledException)
                    {
                        _isAborted = true;
                    }
                    catch (ThreadAbortException)
                    {
                        _isAborted = true;
                    }
                    catch (ThreadInterruptedException)
                    {
                        _isAborted = true;
                    }
                    catch { }
                }
            });

        }
        private bool _isFirst = true;
        private async Task Exe(CancellationToken stoppingToken)
        {
            _manualResetEvent.WaitOne();
            _manualResetEvent.Reset();
            if (_isFirst)
            {
                await _eventBus.RedisHelper.KeyDeleteAsync("Device:" + _dtuId);
                _eventBus.MemoryCache.Remove("Device:" + _dtuId + "$ProductId");
                _isFirst = false;
            }
            var trt = await _eventBus.GetTsl(null, _dtuId, true);
            RawDataMessage rawMsg;
            while (_msgQueue.TryDequeue(out rawMsg))
            {
                string ss = Encoding.UTF8.GetString(rawMsg.Data);
                string[] cmds = ss.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                if (cmds.Length > 0)
                {
                    switch (cmds[0])
                    {
                        case "Read":
                            {
                                int ipin = Convert.ToInt32(cmds[1]);
                                EnablePin(ipin);
                                var pinval = _controller.Read(ipin);
                                await _eventBus.PushReply(_dtuId, (pinval == PinValue.High ? 1 : 0).ToString(), rawMsg.MessageId);
                            }
                            break;
                        case "Write":
                            {
                                int ipin = Convert.ToInt32(cmds[1]);
                                EnablePin(ipin);
                                _controller.Write(ipin, Convert.ToInt32(cmds[2]));
                            }
                            break;
                        case "PinMode":
                            {
                                int ipin = Convert.ToInt32(cmds[1]);
                                EnablePin(ipin);
                                _controller.SetPinMode(ipin, Enum.Parse<PinMode>(cmds[2]));
                            }
                            break;
                        case "Toggle":
                            {
                                int ipin = Convert.ToInt32(cmds[1]);
                                EnablePin(ipin);
                                _controller.Toggle(ipin);
                            }
                            break;
                        case "Event":
                            {
                                int ipin = Convert.ToInt32(cmds[1]);
                                string evtid = cmds[3];
                                _controller.RegisterCallbackForPinValueChangedEvent(ipin, Enum.Parse<PinEventTypes>(cmds[2]), async (object sender, PinValueChangedEventArgs pinValueChangedEventArgs) =>
                                {
                                    Dictionary<string, object> dict = new Dictionary<string, object>();
                                    dict.Add("PinNumber", pinValueChangedEventArgs.PinNumber);
                                    dict.Add("ChangeType", pinValueChangedEventArgs.ChangeType.ToString());
                                    await _eventBus.PublishEvent(trt.ProductId, _dtuId, evtid, dict);
                                });
                            }
                            break;
                    }
                }
            }

        }
        private void EnablePin(int pin)
        {
            if (_openPins.Contains(pin)) { return; }
            _controller.OpenPin(pin);
            _openPins.Add(pin);
        }
        private async Task SuProductHandler(BaseDeviceMessage msg)
        {
            if (_dtuId != msg.DeviceId)
            {
                return;
            }
            byte[] data = null;
            try
            {
                if (msg is RawDataMessage rawMsg)
                {
                    _msgQueue.Enqueue(rawMsg);
                    _manualResetEvent.Set();
                }
            }
            catch (Exception ex)
            {
                await _eventBus.Print(msg.DeviceId, "异常", ex.Message);
            }
        }

        public async Task Stop(CancellationToken cancellationToken)
        {
            _isAborted = true;
            if (_eventBus != null)
            {
                _eventBus.OnSubProductMessage -= SuProductHandler;
            }
            if (_controller != null)
            {
                foreach (var pin in _openPins)
                {
                    _controller.ClosePin(pin);
                }
                _controller.Dispose();
            }

            await _eventBus.Disconnect(_dtuId);
        }
    }
}
