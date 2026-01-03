using ChannelUtility.Message;
using ChannelUtility.Redis;
using ChannelUtility.Tsl;
using EasyNetQ;
using EasyNetQ.Consumer;
using EasyNetQ.DI;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
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
        private IBus _bus;
        public IBus Bus
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
            _bus = RabbitHutch.CreateBus(_option.EventConn, x =>
            {
                x.Register<IConsumerErrorStrategy, ChannelAlwaysRequeueErrorStrategy>();
            });
            _bus.PubSub.Subscribe<string>("IotDown" + _option.config.Code, async (msg, tk) =>
            {
                var rs = System.Text.Json.JsonSerializer.Deserialize<BaseDeviceMessage>(msg, JsonMessageSerializerConfig.DefaultOptions);
                if (OnSubProductMessage != null)
                {
                    await OnSubProductMessage(rs).ConfigureAwait(false);
                }
            }, cfg =>
            {
                cfg.WithTopic("/device." + _option.config.Code + ".down");
                cfg.WithAutoDelete(true);
            });
            _bus.PubSub.Subscribe<string>("IotGuid" + _nodeGuid, async (msg, tk) =>
            {
                var rs = System.Text.Json.JsonSerializer.Deserialize<BaseDeviceMessage>(msg, JsonMessageSerializerConfig.DefaultOptions);
                if (OnSubProductMessage != null)
                {
                    await OnSubProductMessage(rs).ConfigureAwait(false);
                }
            }, cfg =>
            {
                cfg.WithTopic("/device." + _nodeGuid + ".guid");
                cfg.WithAutoDelete(true);
            });

            _bus.PubSub.Subscribe<string>("RuleNode" + Guid.NewGuid().ToString("N"), (msg) =>
            {
                UpdateUpList();
            }, cfg =>
            {
                cfg.WithTopic("/RuleNode.Change");
                cfg.WithAutoDelete(true);
            });
            _bus.PubSub.Subscribe<string>("IotKeyDel" + Guid.NewGuid().ToString("N"), (msg) =>
            {
                string tmpkey = msg;
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

            }, cfg =>
            {
                cfg.WithTopic("/IotKey.Del");
                cfg.WithAutoDelete(true);
            });
            UpdateUpList();
        }

        public async Task DownRequestMessage(RequestMessage msg)
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

            await _bus.PubSub.PublishAsync(data, "/MqttNotice.Msg").ConfigureAwait(false);
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

                            await ConfirmReplyAsync(null, msg).ConfigureAwait(false);
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
            await _bus.PubSub.PublishAsync(System.Text.Json.JsonSerializer.Serialize(msg, JsonMessageSerializerConfig.DefaultOptions), GetUpKey(deviceId));
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
            await _bus.PubSub.PublishAsync(System.Text.Json.JsonSerializer.Serialize(msg, JsonMessageSerializerConfig.DefaultOptions), GetUpKey(deviceId));

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
            await _bus.PubSub.PublishAsync(System.Text.Json.JsonSerializer.Serialize(msg, JsonMessageSerializerConfig.DefaultOptions), GetUpKey(deviceId));
        }
        public async Task PushReply(string deviceId, string value, string msgId = null)
        {
            if (string.IsNullOrEmpty(msgId))
            {
                msgId = await _redis.ListLeftPopAsync<string>($"DeviceMsgId:{deviceId}").ConfigureAwait(false);
            }
            string tkey = "subs:" + deviceId + msgId;
            await _bus.SendReceive.SendAsync("bus.response." + tkey, value).ConfigureAwait(false);
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
            await _bus.PubSub.PublishAsync(System.Text.Json.JsonSerializer.Serialize(msg, JsonMessageSerializerConfig.DefaultOptions), GetUpKey(deviceId));
        }

        /// <summary>
        /// 上报AI检测请求
        /// </summary>
        /// <param name="deviceId"></param>
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
            msg.RgbFrame = frameData;
            msg.Width = width;
            msg.Height = height;
            msg.NodeId = this._nodeGuid;
            await _bus.PubSub.PublishAsync(System.Text.Json.JsonSerializer.Serialize(msg, JsonMessageSerializerConfig.DefaultOptions), GetUpKey(deviceId));
        }

        public void PublishMediaNotFound(string nodeId, string streamId)
        {
            MediaNotFoundMessage msg = new MediaNotFoundMessage();
            msg.DeviceId = this._nodeGuid;
            msg.ProductId = string.Empty;
            msg.NodeId = nodeId;
            msg.StreamId = streamId;
            _bus.PubSub.Publish(System.Text.Json.JsonSerializer.Serialize(msg, JsonMessageSerializerConfig.DefaultOptions), GetUpKey(this._nodeGuid));
        }
        public void PublishMediaNotReader(string streamId)
        {
            MediaNotReaderMessage msg = new MediaNotReaderMessage();
            msg.DeviceId = this._nodeGuid;
            msg.ProductId = string.Empty;
            msg.StreamId = streamId;
            _bus.PubSub.Publish(System.Text.Json.JsonSerializer.Serialize(msg, JsonMessageSerializerConfig.DefaultOptions), GetUpKey(this._nodeGuid));
        }
        /// <summary>
        /// 发送确认回复包（异步）
        /// </summary>
        /// <param name="msgId"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public async Task ConfirmReplyAsync(string msgId, BaseUpDeviceMessage msg)
        {
            if (string.IsNullOrEmpty(msgId))
            {
                await _bus.PubSub.PublishAsync(System.Text.Json.JsonSerializer.Serialize(msg, JsonMessageSerializerConfig.DefaultOptions), GetUpKey(msg.DeviceId));
            }
            else
            {
                await _bus.SendReceive.SendAsync("bus.response." + msgId, msg);
            }
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
