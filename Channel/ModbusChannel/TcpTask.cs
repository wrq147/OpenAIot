using ChannelUtility;
using ChannelUtility.Buffers;
using ChannelUtility.Config;
using ChannelUtility.Message;
using ChannelUtility.Redis;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.IO;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace ModbusChannel
{
    public class TcpTask
    {
        private IServiceProvider _provider;
        private string _ip;
        private int _port;
        private string _dtuId;
        private ClientBusProxy _eventBus;
        private ConcurrentQueue<byte[]> _bytesQueue = new ConcurrentQueue<byte[]>();
        private TcpClient _tcpClient;
        private NetworkStream _tcpStream;
        private Task _mainTask;
        private bool _isAborted;
        private byte[] _readBuffer = new byte[2048];
        private Task _recvTask;
        private CancellationTokenSource _recvCancellationTokenSource;
        private int _down_interval;
        private int _send_interval;
        public TcpTask(string ip, int port, string dtuId, int downInterval, int send_interval, IServiceProvider provider)
        {
            _ip = ip;
            _port = port;
            _dtuId = dtuId;
            _down_interval = downInterval;
            _send_interval = send_interval;
            _provider = provider;
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
        public async Task Start(CancellationToken stoppingToken)
        {
            _tcpClient = new TcpClient();
            _eventBus = _provider.GetService<ClientBusProxy>();
            _eventBus.OnSubProductMessage += SuProductHandler;
            _mainTask = Task.Run(async () =>
            {
                while (!_isAborted)
                {
                    try
                    {
                        await Exe();
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
                if (_tcpStream != null)
                {
                    _tcpStream.Dispose();
                }
                if (_tcpClient != null)
                {
                    _tcpClient.Dispose();
                }
            }
            catch { }
            if (_eventBus != null)
            {
                _eventBus.OnSubProductMessage -= SuProductHandler;
            }
            await _eventBus.Disconnect(_dtuId);
            _bytesQueue.Clear();
        }

        private bool _isOnline = false;
        private HashSet<byte> _hasSendHS = new HashSet<byte>();
        private bool _needDetal = true;
        AutoResetEvent _autoResetEvent = new AutoResetEvent(false);
        AutoResetEvent _delResetEvent = new AutoResetEvent(false);
        private int _delTime = 1000;
        private long _lastLoopTime = 0;
        private int _clearLoopCount = 0;
        private int _loopIdx = 0;


        private long _pplastTime = 0;
        private ConcurrentDictionary<byte, long> _childrenTime = new ConcurrentDictionary<byte, long>();
        private async Task StartRecv(CancellationToken cancellationToken)
        {

            try
            {
                while (!_isAborted && !cancellationToken.IsCancellationRequested)
                {
                    int bytesReadLen = await _tcpStream.ReadAsync(_readBuffer, 0, _readBuffer.Length);
                    if (bytesReadLen == 0)
                    {
                        Console.WriteLine("tcp任务服务端关闭了连接。");
                        break;
                    }
                    // 处理接收到的数据
                    byte[] newbytes = new byte[bytesReadLen];
                    Buffer.BlockCopy(_readBuffer, 0, newbytes, 0, bytesReadLen);
                    await _eventBus.PublishRawUp(_dtuId, newbytes, string.Empty, true);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"tcp任务接收数据出错: {ex.Message}");
            }
            finally
            {
                _needclose = true;
                _recvTask = null;
            }
        }
        private bool _needclose = false;
        private bool _isFirst = true;
        private async Task Exe()
        {
            if (_needclose)
            {
                _needclose = false;
                if (_tcpClient.Client != null)
                {
                    _tcpStream?.Close();
                    _tcpClient?.Close();
                }

            }
            if (_tcpClient.Connected)
            {
                if (_needDetal)
                {
                    _autoResetEvent.WaitOne(_down_interval);
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
                //超30秒无回复离线
                if (deltaSecond > 30000)
                {
                    //执行离线程序
                    if (_isOnline)
                    {
                        await _eventBus.Disconnect(_dtuId);
                        _isOnline = false;
                        return;
                    }
                }
                else
                {
                    if (_isOnline == false)
                    {
                        Console.WriteLine($"设备{_dtuId}上线");
                        if (_isFirst)
                        {
                            await _eventBus.RedisHelper.KeyDeleteAsync("Device:" + _dtuId);
                            _eventBus.MemoryCache.Remove("Device:" + _dtuId + "$ProductId");
                            _isFirst = false;
                        }
                    }
                    _isOnline = true;
                }

                //执行在线程序
                var trt = await _eventBus.GetTsl(null, _dtuId, _isOnline);
                if (trt != null)
                {
                    if (trt.Model.modbus != null)
                    {
                        if (_bytesQueue.Count > 0)
                        {
                            byte[] writebytes;
                            if (_bytesQueue.TryDequeue(out writebytes))
                            {
                                #region 写入待发送数据
                                if (trt.Status == "0")
                                {
                                    await _eventBus.Print(_dtuId, "设备下发消息", FastBufferHelper.ByteToHexStr(writebytes));
                                }
                                await _tcpStream.WriteAsync(writebytes, 0, writebytes.Length);
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
                                            _delResetEvent.WaitOne(_delTime - tmpinittt);
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
                                    if ((curtime - tmpllltime) > 30000)
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
                                        await _eventBus.Print(_dtuId, "设备下发消息", FastBufferHelper.ByteToHexStr(tmpbytes));
                                    }

                                    await _tcpStream.WriteAsync(tmpbytes, 0, tmpbytes.Length);
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
                }
            }
            else
            {
                bool canOpen = true;
                try
                {
                    var trt = await _eventBus.GetTsl(null, _dtuId);
                    if (trt == null)
                    {
                        return;
                    }
                    if (_recvCancellationTokenSource != null)
                    {
                        _recvCancellationTokenSource.Cancel();
                    }
                    //tcp连接
                    if (_tcpClient.Client == null)
                    {
                        _tcpClient = new TcpClient();
                    }

                    Console.WriteLine($"tcp开始连接{_ip}:{_port}");
                    await _tcpClient.ConnectAsync(IPAddress.Parse(_ip), _port);
                    _tcpStream = _tcpClient.GetStream();
                    _recvCancellationTokenSource = new CancellationTokenSource();
                    _recvTask = Task.Run(async () =>
                    {
                        await StartRecv(_recvCancellationTokenSource.Token);
                    });

                    //时间延时
                    _delTime = _send_interval;
                }
                catch (Exception ex)
                {
                    await _eventBus.Print(_dtuId, $"设备{_dtuId}", $"设备{_dtuId}连接异常{ex.Message}");
                    if (_isOnline)
                    {
                        await _eventBus.Disconnect(_dtuId);
                        _isOnline = false;
                    }
                    canOpen = false;
                }
                if (!canOpen)
                {
                    await Task.Delay(20000);
                }

            }
        }
    }
}
