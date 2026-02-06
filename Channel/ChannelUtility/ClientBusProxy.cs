using ChannelUtility.Message;
using ChannelUtility.Redis;
using ChannelUtility.Tsl;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using NATS.Client.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Unicode;
using System.Threading;
using System.Threading.Tasks;


namespace ChannelUtility
{
    /// <summary>
    /// 客户端用事件总线代理
    /// </summary>
    public class ClientBusProxy : IDisposable
    {
        private IServiceProvider _provider;
        public delegate Task SubProductMessage(BaseDeviceMessage msg);
        /// <summary>
        /// 监听订阅的指定协议的消息
        /// </summary>
        public event SubProductMessage OnSubProductMessage;
        private INatsConnection _bus;
        public INatsConnection Bus
        {
            get { return _bus; }
        }
        protected IMemoryCache _memoryCache;
        public IMemoryCache MemoryCache { get { return _memoryCache; } }
        protected GeneralRedisHelper _redis;
        public GeneralRedisHelper RedisHelper { get { return _redis; } }
        protected ChannelOption _option;
        public ChannelOption Option
        {
            get { return _option; }
        }
        private string _nodeId;
        public string NodeId
        {
            get { return _nodeId; }
        }
        private List<IAsyncDisposable> _subscriptions = new List<IAsyncDisposable>();
        private readonly object _subscriptionLock = new object();
        public ClientBusProxy(IServiceProvider provider)
        {
            _provider = provider;
            _option = provider.GetService<ChannelOption>();
            _redis = provider.GetService<GeneralRedisHelper>();
            _nodeId = _option.NodeId;

            _memoryCache = provider.GetService<IMemoryCache>();
            var opts = new NatsOpts
            {
                Url = _option.EventConn,
                AuthOpts = new NatsAuthOpts
                {
                    Username = _option.EventUser,
                    Password = _option.EventPass
                },
                ConnectTimeout = TimeSpan.FromSeconds(5)
            };
            _bus = new NatsConnection(opts);
            _bus.ConnectionOpened += OnClientConnected;
            _bus.ConnectionDisconnected += OnClientDisconnected;
            _ = InitNatsConnectionAsync();
            UpdateUpList();
        }
        private async Task InitNatsConnectionAsync()
        {
            try
            {
                // 显式连接NATS服务器
                await _bus.ConnectAsync().ConfigureAwait(false);
                Console.WriteLine($"[ClientBusProxy] NATS连接成功：{_option.EventConn}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ClientBusProxy] NATS连接失败：{ex.Message}");
                // 可根据业务需求添加重试逻辑
            }

            _redis.HashSet("IotChannels", _option.config.Code, System.Text.Json.JsonSerializer.Serialize(_option.config, JsonMessageSerializerConfig.SerializeOptions));
        }

        private async ValueTask OnClientConnected(object? sender, NatsEventArgs args)
        {
            lock (_subscriptionLock)
            {
                if (_subscriptions.Any())
                {
                    foreach (var sub in _subscriptions)
                    {
                        sub.DisposeAsync().AsTask().Wait(); // 同步清理旧订阅
                    }
                    _subscriptions.Clear();
                }
            }
            var productSub = await _bus.SubscribeCoreAsync("node." + _nodeId, null, ChannelNatsJsonSerializer<string>.Default).ConfigureAwait(false);
            _subscriptions.Add(productSub);

            _ = Task.Run(async () =>
            {
                await foreach (var msg in productSub.Msgs.ReadAllAsync().ConfigureAwait(false))
                {
                    try
                    {
                        if (string.IsNullOrEmpty(msg.Data))
                        {
                            continue;
                        }
                        var rs = System.Text.Json.JsonSerializer.Deserialize<BaseDeviceMessage>(msg.Data, JsonMessageSerializerConfig.DefaultOptions);
                        if (string.IsNullOrEmpty(rs.MessageId) && !string.IsNullOrEmpty(msg.ReplyTo))
                        {
                            rs.MessageId = msg.ReplyTo;
                        }

                        if (OnSubProductMessage != null)
                        {
                            await OnSubProductMessage(rs).ConfigureAwait(false);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.Write(ex.Message);
                    }

                }
            });

            var ruleNodeSub = await _bus.SubscribeCoreAsync("RuleNode.Change", "RuleNode" + Guid.NewGuid().ToString("N"), ChannelNatsJsonSerializer<string>.Default).ConfigureAwait(false);
            _subscriptions.Add(ruleNodeSub);
            _ = Task.Run(async () =>
            {
                await foreach (var msg in ruleNodeSub.Msgs.ReadAllAsync().ConfigureAwait(false))
                {
                    try
                    {
                        UpdateUpList();
                    }
                    catch (Exception ex)
                    {
                        Console.Write(ex.Message);
                    }
                }
            });


            var iotKeyDelSub = await _bus.SubscribeCoreAsync("IotKey.Del", "IotKeyDel" + Guid.NewGuid().ToString("N"), ChannelNatsJsonSerializer<string>.Default).ConfigureAwait(false);
            _subscriptions.Add(iotKeyDelSub);
            _ = Task.Run(async () =>
             {
                 await foreach (var msg in iotKeyDelSub.Msgs.ReadAllAsync().ConfigureAwait(false))
                 {
                     try
                     {
                         if (string.IsNullOrEmpty(msg.Data))
                         {
                             continue;
                         }
                         string tmpkey = msg.Data;
                         if (tmpkey.StartsWith("Device:"))
                         {
                             tmpkey = tmpkey + "$ProductId";
                             _memoryCache.Remove(tmpkey);
                         }
                         else if (tmpkey.StartsWith("Offline:"))
                         {
                             string devid = tmpkey.Split(":")[1];
                             tmpkey = "Device:" + devid + "$ProductId";
                             _memoryCache.Remove(tmpkey);
                         }
                         else
                         {
                             _memoryCache.Remove(tmpkey);
                         }
                     }
                     catch (Exception ex)
                     {
                         Console.Write(ex.Message);
                     }

                 }
             });


            if (_option.EnableAI == true)
            {
                var aiNodeSub = await _bus.SubscribeCoreAsync("AINode.Change", "AINode" + Guid.NewGuid().ToString("N"), ChannelNatsJsonSerializer<string>.Default).ConfigureAwait(false);
                _subscriptions.Add(aiNodeSub);
                _ = Task.Run(async () =>
                {
                    await foreach (var msg in aiNodeSub.Msgs.ReadAllAsync().ConfigureAwait(false))
                    {
                        try
                        {
                            UpdateUpAIList();
                        }
                        catch (Exception ex)
                        {
                            Console.Write(ex.Message);
                        }
                    }
                });
                UpdateUpAIList();
            }


            //发送节点上线
            await this.SendNodeOnline();
        }
        private async Task SendNodeOnline()
        {
            try
            {
                NodeOnlineMessage msg = new NodeOnlineMessage();
                msg.DeviceId = this._nodeId;
                msg.ProductId = string.Empty;
                msg.ChannelCode = _option.config.Code;
                var requestTimeout = TimeSpan.FromSeconds(8);
                var replyMsg = await Bus.RequestAsync<string, string>(GetUpKey(this._nodeId), System.Text.Json.JsonSerializer.Serialize(msg, JsonMessageSerializerConfig.DefaultOptions), null, ChannelNatsJsonSerializer<string>.Default, ChannelNatsJsonSerializer<string>.Default, null, new NatsSubOpts()
                {
                    MaxMsgs = 1,
                    Timeout = requestTimeout,
                    ThrowIfNoResponders = true
                });

                if (replyMsg.Data == "ok")
                {
                    Console.WriteLine("Node Online!");
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("发送节点上线消息失败：" + ex.Message);
            }
            var onlineTimer = new System.Timers.Timer(5000);
            onlineTimer.AutoReset = false;
            onlineTimer.Elapsed += async delegate (object? sender, System.Timers.ElapsedEventArgs e)
            {
                try
                {
                    await this.SendNodeOnline();
                }
                finally
                {
                    onlineTimer.Stop();
                    onlineTimer.Dispose();
                }
            };
            onlineTimer.Start();
        }
        private async ValueTask OnClientDisconnected(object? sender, NatsEventArgs args)
        {
            // 清理现有订阅
            lock (_subscriptionLock)
            {
                if (_subscriptions.Any())
                {
                    foreach (var sub in _subscriptions)
                    {
                        try
                        {
                            sub.DisposeAsync().AsTask().Wait(1000); // 限时清理
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[ClientBusProxy] 清理订阅失败：{ex.Message}");
                        }
                    }
                    _subscriptions.Clear();
                }
            }
        }

        public async Task DownRequestMessage(BaseDeviceMessage msg)
        {
            if (OnSubProductMessage != null)
            {
                await OnSubProductMessage(msg).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// 向设备控制台打印消息
        /// </summary>
        /// <param name="devId"></param>
        /// <param name="tip"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public async Task Print(string devId, string tip, object msg)
        {
            List<string> data = new List<string>();
            data.Add("console/" + devId);
            data.Add(tip + ":" + System.Text.Json.JsonSerializer.Serialize(msg, JsonMessageSerializerConfig.SerializeOptions));

            await Bus.PublishAsync(new NatsMsg<List<string>>()
            {
                Subject = "MqttNotice.Msg",
                Data = data
            }, ChannelNatsJsonSerializer<List<string>>.Default).ConfigureAwait(false);
        }

        /// <summary>
        /// 获取指定协议的物模型
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public async Task<TslReturn> GetTsl(string productId, string deviceId, bool sendconn = false)
        {
            if (string.IsNullOrEmpty(productId))
            {
                if (!_memoryCache.TryGetValue("Device:" + deviceId + "$ProductId", out productId))
                {
                    productId = await _redis.HashGetAsync<string>("Device:" + deviceId, "$ProductId").ConfigureAwait(false);
                    if (string.IsNullOrEmpty(productId))
                    {
                        if (sendconn)
                        {
                            //重新发送连接包
                            await Connected(deviceId).ConfigureAwait(false);
                            int tcount = 0;
                            while (tcount < 5)
                            {
                                await Task.Delay(50).ConfigureAwait(false);
                                productId = await _redis.HashGetAsync<string>("Device:" + deviceId, "$ProductId").ConfigureAwait(false);
                                if (!string.IsNullOrEmpty(productId))
                                {
                                    break;
                                }
                                ++tcount;
                            }

                            if (productId == null)
                            {
                                return null;
                            }
                            else
                            {
                                _memoryCache.Set("Device:" + deviceId + "$ProductId", productId, TimeSpan.FromMinutes(60));
                            }
                        }
                        else
                        {
                            //使用临时设备-协议关联
                            TempProductMessage msg = new TempProductMessage();
                            msg.ProductId = string.Empty;
                            msg.DeviceId = deviceId;
                            msg.Timestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();

                            await PublishAsync(msg).ConfigureAwait(false);
                            int tcount = 0;
                            while (tcount < 5)
                            {
                                await Task.Delay(50).ConfigureAwait(false);
                                productId = await _redis.StringGetAsync<string>("TempDevice:" + deviceId).ConfigureAwait(false);
                                if (productId != null)
                                {
                                    break;
                                }
                                ++tcount;
                            }
                            if (productId == null)
                            {
                                return null;
                            }
                        }
                    }
                    else
                    {
                        _memoryCache.Set("Device:" + deviceId + "$ProductId", productId, TimeSpan.FromMinutes(60));
                    }
                }
            }

            Dictionary<string, string> proDict;
            if (!_memoryCache.TryGetValue("ProductSys:" + productId, out proDict))
            {
                proDict = await _redis.HashGetAllAsync<string>("ProductSys:" + productId);
                if (proDict == null)
                {
                    return null;
                }
                else
                {
                    _memoryCache.Set("ProductSys:" + productId, proDict, TimeSpan.FromMinutes(60));
                }
            }

            string modeltsl;
            if (!proDict.TryGetValue("$ModelTSL", out modeltsl))
            {
                return null;
            }

            string modelscript;
            proDict.TryGetValue("$Script", out modelscript);

            string status;
            proDict.TryGetValue("$Status", out status);

            string netway;
            proDict.TryGetValue("$NetworkWay", out netway);

            var model = TslModel.CreateFrom(modeltsl);
            return new TslReturn(productId, model, status, modelscript, netway);
        }



        private List<string> _upList;
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        public void UpdateUpList()
        {
            var dict = _redis.HashGetAll<string>("RuleExeNodes");
            var tmplist = new List<string>();
            if (dict != null)
            {
                foreach (var item in dict)
                {
                    tmplist.Add(item.Key);
                }
            }

            _lock.EnterWriteLock();
            try
            {
                _upList = tmplist;
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }
        private string GetUpKey(string deviceId)
        {
            _lock.EnterReadLock();
            try
            {
                if (_upList == null || _upList.Count == 0) return "device.up";
                int pos = Math.Abs(deviceId.GetHashCode() % _upList.Count);
                return "device.up." + _upList[pos];
            }
            finally
            {
                _lock.ExitReadLock();
            }

        }


        private List<string> _upAIList;
        private readonly ReaderWriterLockSlim _lockAI = new ReaderWriterLockSlim();

        public void UpdateUpAIList()
        {
            var dict = _redis.HashGetAll<string>("AIExeNodes");
            var tmplist = new List<string>();
            if (dict != null)
            {
                foreach (var item in dict)
                {
                    tmplist.Add(item.Key);
                }
            }

            _lockAI.EnterWriteLock();
            try
            {
                _upAIList = tmplist;
            }
            finally
            {
                _lockAI.ExitWriteLock();
            }
        }
        private string GetUpAIKey(string deviceId)
        {
            _lockAI.EnterReadLock();
            try
            {
                if (_upAIList == null || _upAIList.Count == 0) return "device.ai";
                int pos = Math.Abs(deviceId.GetHashCode() % _upAIList.Count);
                return "device.ai." + _upAIList[pos];
            }
            finally
            {
                _lockAI.ExitReadLock();
            }

        }

        /// <summary>
        /// 发送设备在线给事件总线
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        public async Task Connected(string deviceId, string ip = null)
        {
            DeviceOnlineMessage msg = new DeviceOnlineMessage();
            msg.ProductId = string.Empty;
            msg.DeviceId = deviceId;
            msg.Timestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
            msg.IpAddress = ip;
            msg.NodeId = this._nodeId;
            await _bus.PublishAsync(new NatsMsg<string>()
            {
                Subject = GetUpKey(deviceId),
                Data = System.Text.Json.JsonSerializer.Serialize(msg, JsonMessageSerializerConfig.DefaultOptions)
            }, ChannelNatsJsonSerializer<string>.Default).ConfigureAwait(false);
        }

        /// <summary>
        /// 发送设备离线给事件总线
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public async Task Disconnect(string deviceId)
        {
            await _redis.KeyDeleteAsync($"DeviceMsgId:{deviceId}");
            DeviceOfflineMessage msg = new DeviceOfflineMessage();
            msg.ProductId = string.Empty;
            msg.DeviceId = deviceId;
            msg.Timestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();

            await _bus.PublishAsync(new NatsMsg<string>()
            {
                Subject = GetUpKey(deviceId),
                Data = System.Text.Json.JsonSerializer.Serialize(msg, JsonMessageSerializerConfig.DefaultOptions)
            }, ChannelNatsJsonSerializer<string>.Default).ConfigureAwait(false);

        }
        public async Task PublishRawUp(string deviceId, byte[] data, string prefix, bool needReturn = false)
        {
            RawUpDataMessage msg = new RawUpDataMessage();
            msg.DeviceId = deviceId;
            msg.ProductId = string.Empty;
            msg.Data = data;
            msg.prefix = prefix;
            msg.NodeId = this._nodeId;
            msg.IsReturn = needReturn;

            await _bus.PublishAsync(new NatsMsg<string>()
            {
                Subject = GetUpKey(deviceId),
                Data = System.Text.Json.JsonSerializer.Serialize(msg, JsonMessageSerializerConfig.DefaultOptions)
            }, ChannelNatsJsonSerializer<string>.Default).ConfigureAwait(false);
        }

        /// <summary>
        /// 上报事件
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="deviceId"></param>
        /// <param name="eventId"></param>
        /// <param name="outputs"></param>
        /// <returns></returns>
        public async Task PublishEvent(string productId, string deviceId, string eventId, IDictionary<string, object> outputs)
        {
            DeviceEventMessage msg = new DeviceEventMessage();
            msg.DeviceId = deviceId;
            msg.ProductId = productId;
            msg.EventId = eventId;
            msg.Outputs = outputs;
            msg.Timestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();

            await _bus.PublishAsync(new NatsMsg<string>()
            {
                Subject = GetUpKey(deviceId),
                Data = System.Text.Json.JsonSerializer.Serialize(msg, JsonMessageSerializerConfig.DefaultOptions)
            }, ChannelNatsJsonSerializer<string>.Default).ConfigureAwait(false);
        }




        /// <summary>
        /// 上报AI检测请求
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="videoKey"></param>
        /// <param name="motionRatio"></param>
        /// <param name="frameData"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="configs"></param>
        /// <returns></returns>
        public async Task PublishAIDetectRequest(string deviceId, string videoKey, float motionRatio, byte[] frameData, int width, int height, List<AIConfigData> configs)
        {
            AIDetectRequestMeesage msg = new AIDetectRequestMeesage();
            msg.DeviceId = deviceId;
            msg.ProductId = string.Empty;
            msg.MRatio = motionRatio;
            msg.Frame = Encoding.UTF8.GetString(frameData);
            msg.Width = width;
            msg.Height = height;
            msg.NodeId = this._nodeId;
            msg.VideoKey = videoKey;
            msg.Configs = configs;

            await _bus.PublishAsync(new NatsMsg<string>()
            {
                Subject = GetUpAIKey(deviceId),
                Data = System.Text.Json.JsonSerializer.Serialize(msg, JsonMessageSerializerConfig.DefaultOptions)
            }, ChannelNatsJsonSerializer<string>.Default).ConfigureAwait(false);
        }
        public async Task<string> WaitPublishMediaUserVerify(string username)
        {
            try
            {
                MediaUserVerifyMessage msg = new MediaUserVerifyMessage();
                msg.DeviceId = this._nodeId;
                msg.ProductId = string.Empty;
                msg.UserName = username;
                var requestTimeout = TimeSpan.FromSeconds(8);
                var replyMsg = await Bus.RequestAsync<string, string>(GetUpKey(this._nodeId), System.Text.Json.JsonSerializer.Serialize(msg, JsonMessageSerializerConfig.DefaultOptions), null, ChannelNatsJsonSerializer<string>.Default, ChannelNatsJsonSerializer<string>.Default, null, new NatsSubOpts()
                {
                    MaxMsgs = 1,
                    Timeout = requestTimeout,
                    ThrowIfNoResponders = true
                });

                return replyMsg.Data;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async void PublishMediaNotFound(string streamId, int videoType)
        {
            MediaNotFoundMessage msg = new MediaNotFoundMessage();
            msg.DeviceId = this._nodeId;
            msg.ProductId = string.Empty;
            msg.StreamId = streamId;
            msg.VideoType = videoType;

            await _bus.PublishAsync(new NatsMsg<string>()
            {
                Subject = GetUpKey(this._nodeId),
                Data = System.Text.Json.JsonSerializer.Serialize(msg, JsonMessageSerializerConfig.DefaultOptions)
            }, ChannelNatsJsonSerializer<string>.Default).ConfigureAwait(false);
        }
        public async void PublishMediaNotReader(string streamId, int videoType)
        {
            MediaNotReaderMessage msg = new MediaNotReaderMessage();
            msg.DeviceId = this._nodeId;
            msg.ProductId = string.Empty;
            msg.StreamId = streamId;
            msg.VideoType = videoType;

            await _bus.PublishAsync(new NatsMsg<string>()
            {
                Subject = GetUpKey(this._nodeId),
                Data = System.Text.Json.JsonSerializer.Serialize(msg, JsonMessageSerializerConfig.DefaultOptions)
            }, ChannelNatsJsonSerializer<string>.Default).ConfigureAwait(false);
        }
        public async void PublishMediaChannels(string username, List<ChannelData> channelDatas)
        {
            MediaChannelMessage msg = new MediaChannelMessage();
            msg.DeviceId = this._nodeId;
            msg.ProductId = string.Empty;
            msg.UserName = username;
            msg.Channels = channelDatas;

            await _bus.PublishAsync(new NatsMsg<string>()
            {
                Subject = GetUpKey(this._nodeId),
                Data = System.Text.Json.JsonSerializer.Serialize(msg, JsonMessageSerializerConfig.DefaultOptions)
            }, ChannelNatsJsonSerializer<string>.Default).ConfigureAwait(false);
        }
        public async Task PublishMediaPresetReply(string msgId, string dtuId, string username, List<PresetInfo> data)
        {
            MediaPresetMessageReply msg = new MediaPresetMessageReply();
            msg.DeviceId = dtuId;
            msg.ProductId = string.Empty;
            msg.UserName = username;
            msg.Presets = data;

            await _bus.PublishAsync(new NatsMsg<string>()
            {
                Subject = msgId,
                Data = System.Text.Json.JsonSerializer.Serialize(msg, JsonMessageSerializerConfig.DefaultOptions)
            }, ChannelNatsJsonSerializer<string>.Default).ConfigureAwait(false);
        }
        public async Task PublishMediaPTZReply(string msgId, string dtuId, bool isSuccess, string reason)
        {
            MediaPTZMessageReply msg = new MediaPTZMessageReply();
            msg.DeviceId = dtuId;
            msg.ProductId = string.Empty;
            msg.IsSuccess = isSuccess;
            msg.Reason = reason;

            await _bus.PublishAsync(new NatsMsg<string>()
            {
                Subject = msgId,
                Data = System.Text.Json.JsonSerializer.Serialize(msg, JsonMessageSerializerConfig.DefaultOptions)
            }, ChannelNatsJsonSerializer<string>.Default).ConfigureAwait(false);
        }
        public async Task PublishRecordStartReply(string msgId, string dtuId, bool isSuccess, string reason)
        {
            MediaRecordStartMessageReply msg = new MediaRecordStartMessageReply();
            msg.DeviceId = dtuId;
            msg.ProductId = string.Empty;
            msg.IsSuccess = isSuccess;
            msg.Reason = reason;
            await _bus.PublishAsync(new NatsMsg<string>()
            {
                Subject = msgId,
                Data = System.Text.Json.JsonSerializer.Serialize(msg, JsonMessageSerializerConfig.DefaultOptions)
            }, ChannelNatsJsonSerializer<string>.Default).ConfigureAwait(false);
        }

        public async Task PublishRecordStopReply(string msgId, string dtuId, bool isSuccess, string reason)
        {
            MediaRecordStopMessageReply msg = new MediaRecordStopMessageReply();
            msg.DeviceId = dtuId;
            msg.ProductId = string.Empty;
            msg.IsSuccess = isSuccess;
            msg.Reason = reason;
            await _bus.PublishAsync(new NatsMsg<string>()
            {
                Subject = msgId,
                Data = System.Text.Json.JsonSerializer.Serialize(msg, JsonMessageSerializerConfig.DefaultOptions)
            }, ChannelNatsJsonSerializer<string>.Default).ConfigureAwait(false);
        }

        public async Task PublishRecordFile(string dtuId, string streamId, string fileName, ulong fileSize, ulong startTime, float timeLen, byte storage, byte saveType)
        {
            MediaRecordFileMessage msg = new MediaRecordFileMessage();
            msg.DeviceId = dtuId;
            msg.StreamId = streamId;
            msg.FileName = fileName;
            msg.FileSize = fileSize;
            msg.StartTime = startTime;
            msg.TimeLen = timeLen;
            msg.NodeId = this._nodeId;
            msg.Storage = storage;
            msg.SaveType = saveType;
            await _bus.PublishAsync(new NatsMsg<string>()
            {
                Subject = GetUpKey(msg.DeviceId),
                Data = System.Text.Json.JsonSerializer.Serialize(msg, JsonMessageSerializerConfig.DefaultOptions)
            }, ChannelNatsJsonSerializer<string>.Default).ConfigureAwait(false);
        }
        /// <summary>
        /// 发送指定消息
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public async Task PublishAsync(BaseUpDeviceMessage msg)
        {
            await _bus.PublishAsync(new NatsMsg<string>()
            {
                Subject = GetUpKey(msg.DeviceId),
                Data = System.Text.Json.JsonSerializer.Serialize(msg, JsonMessageSerializerConfig.DefaultOptions)
            }, ChannelNatsJsonSerializer<string>.Default).ConfigureAwait(false);
        }

        public async Task PublishReply(string msgId, string data)
        {
            await _bus.PublishAsync(new NatsMsg<string>()
            {
                Subject = msgId,
                Data = data
            }, ChannelNatsJsonSerializer<string>.Default).ConfigureAwait(false);
        }

        //供程序员显式调用的Dispose方法
        public void Dispose()
        {
            //调用带参数的Dispose方法，释放托管和非托管资源
            Dispose(true);
            //手动调用了Dispose释放资源，那么析构函数就是不必要的了，这里阻止GC调用析构函数
            System.GC.SuppressFinalize(this);
        }

        //protected的Dispose方法，保证不会被外部调用。
        //传入bool值disposing以确定是否释放托管资源
        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                //TODO:在这里加入清理"托管资源"的代码，应该是xxx.Dispose();
            }
            //TODO:在这里加入清理"非托管资源"的代码
        }

        //供GC调用的析构函数
        ~ClientBusProxy()
        {
            Dispose(false);//释放非托管资源
        }
    }
}
