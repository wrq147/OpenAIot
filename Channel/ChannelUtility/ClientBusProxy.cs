using ChannelUtility.Buffers;
using ChannelUtility.Message;
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
    public class ClientBusProxy : ChannelRegister
    {
        private IServiceProvider _provider;
        public delegate Task SubProductMessage(RequestMessage msg);
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
        public ClientBusProxy(IServiceProvider provider) : base(provider)
        {
            _provider = provider;
            _memoryCache = provider.GetService<IMemoryCache>();
            _bus = RabbitHutch.CreateBus(_option.EventConn, x =>
            {
                x.Register<IConsumerErrorStrategy, ChannelAlwaysRequeueErrorStrategy>();
            });
            _bus.PubSub.Subscribe<string>("IotDown" + _option.config.Code, async (msg, tk) =>
            {
                var rs = System.Text.Json.JsonSerializer.Deserialize<RequestMessage>(msg, JsonMessageSerializerConfig.DefaultOptions);
                if (OnSubProductMessage != null)
                {
                    await OnSubProductMessage(rs).ConfigureAwait(false);
                }
            }, cfg =>
            {
                cfg.WithTopic("/device." + _option.config.Code + ".down");
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
        public async Task PublishRawUp(string deviceId, byte[] data, string prefix)
        {
            RawUpDataMessage msg = new RawUpDataMessage();
            msg.DeviceId = deviceId;
            msg.ProductId = string.Empty;
            msg.Data = data;
            msg.prefix = prefix;
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


    }
}
