using System;
using System.Linq;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ChannelUtility;
using System.Collections.Generic;
using ChannelUtility.Buffers;
using ChannelUtility.Message;
using ChannelUtility.Redis;
using ChannelUtility.Config;
using System.Collections.Concurrent;
using System.IO;

namespace ModbusChannel
{
    public class SerialTask
    {
        private Task _mainTask;
        private bool _isAborted;
        private SerialItem _item;
        private IServiceProvider _provider;
        private ClientBusProxy _eventBus;
        private SerialPort _port;
        private ConcurrentQueue<byte[]> _bytesQueue = new ConcurrentQueue<byte[]>();
        private Task _recvTask;
        private CancellationTokenSource _recvCancellationTokenSource;
        private ModbusOption _option;
        public SerialTask(SerialItem item, ModbusOption option, IServiceProvider provider)
        {
            _option = option;
            _item = item;
            _provider = provider;
        }

        private async Task SuProductHandler(BaseDeviceMessage msg)
        {
            if (_item.dtuid != msg.DeviceId)
            {
                return;
            }
            byte[] data = null;
            try
            {
                if (msg is RawDataMessage rawMsg)
                {
                    data = rawMsg.Data;
                }
                else if (msg is ModbusMatchMessage mmsg)
                {
                    var curttt = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
                    _pplastTime = curttt;
                    foreach (var mm in mmsg.MatchList)
                    {
                        _childrenTime.AddOrUpdate(mm.SlaveId, curttt, (k, v) => curttt);
                    }
                    _autoResetEvent.Set();
                }
                if (data != null && data.Length > 0)
                {
                    _bytesQueue.Enqueue(data);
                    _delResetEvent.Set();
                }

            }
            catch (Exception ex)
            {
                await _eventBus.Print(msg.DeviceId, "异常", ex.Message);
            }
        }

        private long _pplastTime = 0;
        private ConcurrentDictionary<byte, long> _childrenTime = new ConcurrentDictionary<byte, long>();
        private byte[] _readBuffer = new byte[2048];
        private byte[] _lastBytes;
        private long _lastByteTime = 0;
        private bool _needclose = false;
        private async Task<int> ReadWithTimeout(Stream stream, byte[] buffer, int offset, int count, int timeoutMs, CancellationToken cancellationToken)
        {
            using (var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
            {
                cts.CancelAfter(timeoutMs);
                try
                {
                    // 尝试读取数据，带超时和外部取消支持
                    return await stream.ReadAsync(buffer, offset, count, cts.Token);
                }
                catch (OperationCanceledException)
                {
                    // 超时或外部取消时返回-1（区分正常0字节）
                    return -1;
                }
                catch (IOException)
                {
                    // 明确的IO异常（如端口断开）返回-2
                    return -2;
                }
            }
        }

        private async Task StartRecv(CancellationToken cancellationToken)
        {
            try
            {
                int consecutiveTimeouts = 0; // 连续超时计数器
                while (!_isAborted && !cancellationToken.IsCancellationRequested)
                {
                    if (_port == null || !_port.IsOpen)
                    {
                        Console.WriteLine($"{_item.dtuid}串口已关闭，退出监听");
                        break;
                    }
                    int bytesReadLen = await ReadWithTimeout(
                        _port.BaseStream,
                        _readBuffer,
                        0,
                        _readBuffer.Length,
                        12000,  // 超时12秒
                        cancellationToken
                    );

                    if (bytesReadLen == -2)
                    {
                        // 明确的IO异常（如物理断开），直接退出
                        Console.WriteLine($"串口{_item.dtuid}连接已中断（IO异常）");
                        break;
                    }
                    else if (bytesReadLen == -1)
                    {
                        consecutiveTimeouts++;
                        Console.WriteLine($"串口{_item.dtuid}读取超时（{consecutiveTimeouts}次）");

                        // 达到最大连续超时次数，判定为连接中断
                        if (consecutiveTimeouts >= 3)
                        {
                            Console.WriteLine($"{_item.dtuid}连续超时次数达到阈值，判定连接中断");
                            break;
                        }
                        continue;
                    }
                    else
                    {
                        // 读取到数据：重置超时计数器
                        consecutiveTimeouts = 0;
                    }

                    if (bytesReadLen > 0)
                    {
                        var curttt = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
                        // 处理接收到的数据
                        byte[] tmpbytes;

                        if (_lastBytes != null && _lastBytes.Length > 0 && (curttt - _lastByteTime) < 200)
                        {
                            // 计算总长度并直接创建目标数组
                            int totalLength = _lastBytes.Length + bytesReadLen;
                            tmpbytes = new byte[totalLength];

                            // 一次完成两段数据复制
                            Buffer.BlockCopy(_lastBytes, 0, tmpbytes, 0, _lastBytes.Length);
                            Buffer.BlockCopy(_readBuffer, 0, tmpbytes, _lastBytes.Length, bytesReadLen);
                        }
                        else
                        {
                            tmpbytes = new byte[bytesReadLen];
                            Buffer.BlockCopy(_readBuffer, 0, tmpbytes, 0, bytesReadLen);
                        }
                        if (tmpbytes.Length <= 5)
                        {
                            _lastByteTime = curttt;
                            _lastBytes = tmpbytes;
                            continue;
                        }
                        await _eventBus.PublishRawUp(_item.dtuid, tmpbytes, string.Empty, true);
                        _lastBytes = null;
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"tcp任务接收数据出错: {ex.Message}");
            }
            finally
            {
                _needclose = true;
            }
        }
        public async Task Start(CancellationToken stoppingToken)
        {
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
        public async Task Stop()
        {
            _isAborted = true;
            try
            {
                if (_port != null)
                {
                    _port.Dispose();
                }

            }
            catch { }
            if (_eventBus != null)
            {
                _eventBus.OnSubProductMessage -= SuProductHandler;
            }
            await Offline();
            _bytesQueue.Clear();
        }
        private int _loopIdx = 0;

        private async Task Offline()
        {
            Console.WriteLine($"设备{_item.dtuid}离线");
            //上报设备离线
            await _eventBus.Disconnect(_item.dtuid);
        }

        private bool _isOnline = false;
        private HashSet<byte> _hasSendHS = new HashSet<byte>();
        private bool _needDetal = true;
        AutoResetEvent _autoResetEvent = new AutoResetEvent(false);
        AutoResetEvent _delResetEvent = new AutoResetEvent(false);
        private int _delTime = 1000;
        private string _preModbusStr = string.Empty;
        private long _lastLoopTime = 0;
        private int _clearLoopCount = 0;
        private int _cmpccc = 1;
        private async Task Exe(CancellationToken stoppingToken)
        {
            if (_needclose)
            {
                _needclose = false;
                if (_port != null && _port.IsOpen)
                {
                    _port.Dispose();
                    _port = null;
                    return;
                }
            }
            if (_port != null && _port.IsOpen)
            {
                if (_needDetal)
                {
                    _autoResetEvent.WaitOne(_option.down_interval);
                }
                else
                {
                    _needDetal = true;
                }

                //时间间隔
                if (_pplastTime <= 0)
                {
                    _pplastTime = new DateTimeOffset(DateTime.Now).AddHours(-1).ToUnixTimeMilliseconds();
                }
                long curtime = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
                long deltaSecond = curtime - _pplastTime;
                //超60秒无回复离线
                if (deltaSecond > 60000)
                {
                    //执行离线程序
                    if (_isOnline)
                    {
                        _isOnline = false;
                        await Offline();
                        return;
                    }
                }
                else
                {
                    if (_isOnline == false)
                    {
                        Console.WriteLine($"设备{_item.dtuid}上线");
                        //开机确保上线
                        await _eventBus.RedisHelper.KeyDeleteAsync("Device:" + _item.dtuid);
                        _eventBus.MemoryCache.Remove("Device:" + _item.dtuid + "$ProductId");
                    }
                    _isOnline = true;
                }

                //执行在线程序
                var trt = await _eventBus.GetTsl(null, _item.dtuid, _isOnline);
                if (trt != null)
                {
                    if (trt.Model.modbus != null)
                    {
                        ++_cmpccc;
                        _cmpccc = _cmpccc % 20;
                        if (_cmpccc == 0)
                        {
                            var newmodbusstr = $"{trt.Model.modbus.BaudRate}|{trt.Model.modbus.DataBits}|{trt.Model.modbus.Parity}|{trt.Model.modbus.StopBits}";
                            if (!string.Equals(_preModbusStr, newmodbusstr))
                            {
                                _port.Dispose();
                                _port = null;
                                return;
                            }
                        }

                        if (_bytesQueue.Count > 0)
                        {
                            byte[] writebytes;
                            if (_bytesQueue.TryDequeue(out writebytes))
                            {
                                #region 写入待发送数据
                                if (trt.Status == "0")
                                {
                                    await _eventBus.Print(_item.dtuid, "设备下发消息", FastBufferHelper.ByteToHexStr(writebytes));
                                }
                                try
                                {
                                    _port.Write(writebytes, 0, writebytes.Length);
                                }
                                catch (Exception ex)
                                {
                                    _port.Dispose();
                                    _port = null;
                                    return;
                                }
                                #endregion
                            }
                        }
                        else
                        {
                            #region 发送采集数据请求
                            if (trt.Model.modbus.Matches.Count > 0)
                            {
                                if (trt.Model.modbus.Matches.Count > _loopIdx)
                                {
                                    var itemIdx = _loopIdx;
                                    var mm = trt.Model.modbus.Matches[_loopIdx];
                                    long tmpllltime = _childrenTime.GetOrAdd(mm.SlaveId, (k) => new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds());
                                    if (_loopIdx == 0)
                                    {
                                        long tmpdddd = curtime - _lastLoopTime;
                                        if (tmpdddd < _delTime)
                                        {
                                            int tmpinittt = (int)tmpdddd;
                                            _delResetEvent.WaitOne(Math.Max(0, _delTime - tmpinittt));
                                            _needDetal = false;
                                            return;
                                        }
                                        ++_clearLoopCount;
                                        if (_clearLoopCount > 10)
                                        {
                                            _clearLoopCount = 0;
                                            _hasSendHS.Clear();
                                        }
                                    }
                                    if ((curtime - tmpllltime) > 60000)
                                    {
                                        if (_hasSendHS.Contains(mm.SlaveId))
                                        {
                                            _loopIdx = (_loopIdx + 1) % trt.Model.modbus.Matches.Count;
                                            _needDetal = false;
                                            if (_loopIdx == 0)
                                            {
                                                _lastLoopTime = curtime;
                                            }
                                            return;
                                        }
                                        _hasSendHS.Add(mm.SlaveId);
                                    }

                                    byte[] tmpbytes = FastBufferHelper.ModbusMatch2Bytes(trt.Model.modbus.Mode, mm, itemIdx);
                                    if (trt.Status == "0")
                                    {
                                        await _eventBus.Print(_item.dtuid, "设备下发消息", FastBufferHelper.ByteToHexStr(tmpbytes));
                                    }
                                    try
                                    {
                                        _port.Write(tmpbytes, 0, tmpbytes.Length);
                                    }
                                    catch (Exception ex)
                                    {
                                        _port.Dispose();
                                        _port = null;
                                        return;
                                    }

                                    _loopIdx = (_loopIdx + 1) % trt.Model.modbus.Matches.Count;
                                    if (_loopIdx == 0)
                                    {
                                        _lastLoopTime = curtime;
                                    }
                                }
                                else
                                {
                                    _loopIdx = 0;
                                }
                            }
                            #endregion
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(_preModbusStr))
                        {
                            _port.Dispose();
                            _port = null;
                            return;
                        }
                    }
                }
            }
            else
            {
                bool canOpen = true;
                try
                {
                    if (_recvCancellationTokenSource != null)
                    {
                        _recvCancellationTokenSource.Cancel();
                    }
                    if (_port != null)
                    {
                        _port.Dispose();
                    }
                    _port = new SerialPort(_item.name);
                    var trt = await _eventBus.GetTsl(null, _item.dtuid);
                    if (trt == null || trt.Model == null)
                    {
                        throw new Exception($"{_item.dtuid}物模型不存在");
                    }
                    if (trt.Model.modbus == null)
                    {
                        _preModbusStr = string.Empty;
                        _port.BaudRate = 9600;//波特率
                        _port.DataBits = 8;//数据位
                        _port.Parity = Parity.None;//校验位
                        _port.StopBits = StopBits.One;//停止位
                    }
                    else
                    {
                        _preModbusStr = $"{trt.Model.modbus.BaudRate}|{trt.Model.modbus.DataBits}|{trt.Model.modbus.Parity}|{trt.Model.modbus.StopBits}";
                        _port.BaudRate = trt.Model.modbus.BaudRate;//波特率
                        _port.DataBits = trt.Model.modbus.DataBits;//数据位
                                                                   //奇偶效验
                        switch (trt.Model.modbus.Parity)
                        {
                            case "0":
                                _port.Parity = Parity.None;
                                break;
                            case "1":
                                _port.Parity = Parity.Odd;
                                break;
                            case "2":
                                _port.Parity = Parity.Even;
                                break;
                        }
                        _port.StopBits = trt.Model.modbus.StopBits == "1" ? StopBits.One : StopBits.Two;//停止位
                    }
                    _port.Open();
                    _delTime = _option.send_interval;
                    _recvCancellationTokenSource = new CancellationTokenSource();

                    _recvTask = Task.Run(async () =>
                    {
                        await StartRecv(_recvCancellationTokenSource.Token);
                    });

                    await _eventBus.Print(_item.dtuid, "设备端口", $"设备端口{_item.dtuid}连接");
                }
                catch (Exception ex)
                {
                    await _eventBus.Print(_item.dtuid, "设备端口异常", $"设备{_item.dtuid}的端口异常{ex.Message}");
                    Console.WriteLine("设备端口异常:" + ex.Message);
                    if (ex.Message.Contains("async-ops"))
                    {
                        await _eventBus.RedisHelper.ReConnectAsync();
                    }
                    if (_isOnline)
                    {
                        _isOnline = false;
                        await Offline();
                    }
                    canOpen = false;
                }
                if (!canOpen)
                {
                    await Task.Delay(20000, stoppingToken);
                }
            }
        }
    }
}
