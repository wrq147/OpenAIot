using ChannelUtility.Message;
using ChannelUtility.Redis;
using ChannelUtility.Tsl;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using NATS.Client.Core;
using System;
using System.Collections.Generic;
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
        private string _nodeGuid;
        public string NodeGuid
        {
            get { return _nodeGuid; }
        }
        public ClientBusProxy(IServiceProvider provider)
        {
            _provider = provider;
            _option = provider.GetService<ChannelOption>();
            _redis = provider.GetService<GeneralRedisHelper>();
            _nodeGuid = "NC" + Guid.NewGuid().ToString("N");

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
            InitBus();

            UpdateUpList();
        }
        private void InitBus()
        {
            Task.Run(async () =>
            {
                await foreach (var msg in _bus.SubscribeAsync("/device." + _option.config.Code + ".down", "IotDown" + _option.config.Code, ChannelNatsJsonSerializer<string>.Default).ConfigureAwait(false))
                {
                    if (string.IsNullOrEmpty(msg.Data))
                    {
                        continue;
                    }
                    
                    var rs = System.Text.Json.JsonSerializer.Deserialize<BaseDeviceMessage>(msg.Data, JsonMessageSerializerConfig.DefaultOptions);
                    if (!string.IsNullOrEmpty(msg.ReplyTo))
                    {
                        rs.MessageId = msg.ReplyTo;
                    }
  
                    if (OnSubProductMessage != null)
                    {
                        await OnSubProductMessage(rs).ConfigureAwait(false);
                    }
                }
            });
            Task.Run(async () =>
            {
                await foreach (var msg in _bus.SubscribeAsync("/node." + _nodeGuid, "IotGuid" + _nodeGuid, ChannelNatsJsonSerializer<string>.Default).ConfigureAwait(false))
                {
                    if (string.IsNullOrEmpty(msg.Data))
                    {
                        continue;
                    }
                    var rs = System.Text.Json.JsonSerializer.Deserialize<BaseDeviceMessage>(msg.Data, JsonMessageSerializerConfig.DefaultOptions);
                    if (!string.IsNullOrEmpty(msg.ReplyTo))
                    {
                        rs.MessageId = msg.ReplyTo;
                    }

                    if (OnSubProductMessage != null)
                    {
                        await OnSubProductMessage(rs).ConfigureAwait(false);
                    }
                }
            });

            Task.Run(async () =>
            {
                await foreach (var msg in _bus.SubscribeAsync("/RuleNode.Change", "RuleNode" + Guid.NewGuid().ToString("N"), ChannelNatsJsonSerializer<string>.Default).ConfigureAwait(false))
                {
                    UpdateUpList();
                }
            });

            Task.Run(async () =>
            {
                await foreach (var msg in _bus.SubscribeAsync("/IotKey.Del", "IotKeyDel" + Guid.NewGuid().ToString("N"), ChannelNatsJsonSerializer<string>.Default).ConfigureAwait(false))
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
            });

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
                Subject = "/MqttNotice.Msg",
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
                if (_upList == null || _upList.Count == 0) return "/device.up";
                int pos = Math.Abs(deviceId.GetHashCode() % _upList.Count);
                return "/device.up." + _upList[pos];
            }
            finally
            {
                _lock.ExitReadLock();
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
        public async Task PublishRawUp(string deviceId, byte[] data, string prefix, bool enableNodeId = false)
        {
            RawUpDataMessage msg = new RawUpDataMessage();
            msg.DeviceId = deviceId;
            msg.ProductId = string.Empty;
            msg.Data = data;
            msg.prefix = prefix;
            if (enableNodeId)
            {
                msg.NodeId = this._nodeGuid;
            }
            else
            {
                msg.NodeId = string.Empty;
            }

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
        /// <param name="streamId"></param>
        /// <param name="detectType"></param>
        /// <param name="detParams"></param>
        /// <param name="isDraw"></param>
        /// <param name="frameData"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public async Task PublishAIDetectRequest(string deviceId, string detectType, Dictionary<string, object> detParams, bool isDraw, byte[] frameData, int width, int height)
        {
            AIDetectRequestMeesage msg = new AIDetectRequestMeesage();
            msg.DeviceId = deviceId;
            msg.ProductId = string.Empty;
            msg.DetType = detectType;
            msg.IsDraw = isDraw;
            msg.DetParams = detParams;
            msg.Frame = frameData;
            msg.Width = width;
            msg.Height = height;
            msg.NodeId = this._nodeGuid;

            await _bus.PublishAsync(new NatsMsg<string>()
            {
                Subject = GetUpKey(deviceId),
                Data = System.Text.Json.JsonSerializer.Serialize(msg, JsonMessageSerializerConfig.DefaultOptions)
            }, ChannelNatsJsonSerializer<string>.Default).ConfigureAwait(false);
        }
        public async Task<string> WaitPublishMediaUserVerify(string nodeId, string username)
        {
            try
            {
                MediaUserVerifyMessage msg = new MediaUserVerifyMessage();
                msg.DeviceId = nodeId;
                msg.ProductId = string.Empty;
                msg.UserName = username;
                var requestTimeout = TimeSpan.FromSeconds(8);
                var replyMsg = await Bus.RequestAsync<string, string>(GetUpKey(nodeId), System.Text.Json.JsonSerializer.Serialize(msg, JsonMessageSerializerConfig.DefaultOptions), null, ChannelNatsJsonSerializer<string>.Default, ChannelNatsJsonSerializer<string>.Default, null, new NatsSubOpts()
                {
                    MaxMsgs = 1,
                    Timeout = requestTimeout,
                    StartUpTimeout = requestTimeout,
                    ThrowIfNoResponders = true
                });

                return replyMsg.Data;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async void PublishMediaNotFound(string nodeId, string streamId, int videoType)
        {
            MediaNotFoundMessage msg = new MediaNotFoundMessage();
            msg.DeviceId = nodeId;
            msg.ProductId = string.Empty;
            msg.StreamId = streamId;
            msg.NodeGuid = this._nodeGuid;
            msg.VideoType = videoType;

            await _bus.PublishAsync(new NatsMsg<string>()
            {
                Subject = GetUpKey(nodeId),
                Data = System.Text.Json.JsonSerializer.Serialize(msg, JsonMessageSerializerConfig.DefaultOptions)
            }, ChannelNatsJsonSerializer<string>.Default).ConfigureAwait(false);

        }
        public async void PublishMediaNotReader(string nodeId, string streamId, int videoType)
        {
            MediaNotReaderMessage msg = new MediaNotReaderMessage();
            msg.DeviceId = nodeId;
            msg.ProductId = string.Empty;
            msg.StreamId = streamId;
            msg.VideoType = videoType;

            await _bus.PublishAsync(new NatsMsg<string>()
            {
                Subject = GetUpKey(nodeId),
                Data = System.Text.Json.JsonSerializer.Serialize(msg, JsonMessageSerializerConfig.DefaultOptions)
            }, ChannelNatsJsonSerializer<string>.Default).ConfigureAwait(false);
        }
        public async void PublishMediaChannels(string nodeId, string username, List<ChannelData> channelDatas)
        {
            MediaChannelMessage msg = new MediaChannelMessage();
            msg.DeviceId = nodeId;
            msg.ProductId = string.Empty;
            msg.UserName = username;
            msg.NodeGuid = this._nodeGuid;
            msg.Channels = channelDatas;

            await _bus.PublishAsync(new NatsMsg<string>()
            {
                Subject = GetUpKey(nodeId),
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
