using ChannelUtility;
using ChannelUtility.Config;
using ChannelUtility.Message;
using Common.EventBus;
using Common.Share;
using IoTService.DAL;
using IoTService.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NATS.Client.Core;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TemplateAction.Core;



namespace IoTService
{
    /// <summary>
    /// 服务端用事件总线代理
    /// </summary>
    public class ServerBusProxy
    {
        private ITAServiceProvider _provider;
        private ILogger<ServerBusProxy> _log;
        public ServerBusProxy(ITAServiceProvider provider, ILoggerFactory logFactory)
        {
            _provider = provider;
            _log = logFactory.CreateLogger<ServerBusProxy>();
        }
        public async Task PublishKeyDel(string key)
        {
            var bus = _provider.GetService<NatsScope>().Bus;
            await bus.PublishAsync(new NatsMsg<string>()
            {
                Subject = "/IotKey.Del",
                Data = key
            }, DefalutNatsJsonSerializer<string>.Default);
        }
        public async Task PublishNodeChange()
        {
            var bus = _provider.GetService<NatsScope>().Bus;
            await bus.PublishAsync(new NatsMsg<string>()
            {
                Subject = "/RuleNode.Change",
                Data = string.Empty
            }, DefalutNatsJsonSerializer<string>.Default);
        }
        public async Task Print(string devId, string tip, object msg)
        {
            var bus = _provider.GetService<NatsScope>().Bus;

            List<string> data = new List<string>();
            data.Add("console/" + devId);
            data.Add(tip + ":" + System.Text.Json.JsonSerializer.Serialize(msg, JsonMessageSerializerConfig.SerializeOptions));
            await bus.PublishAsync(new NatsMsg<List<string>>()
            {
                Subject = "/MqttNotice.Msg",
                Data = data
            }, DefalutNatsJsonSerializer<List<string>>.Default);
        }
        public async Task NoticeChange(string devId, string param = "")
        {
            var bus = _provider.GetService<NatsScope>().Bus;

            List<string> data = new List<string>();
            data.Add("newprop/" + devId);
            data.Add(param);

            await bus.PublishAsync(new NatsMsg<List<string>>()
            {
                Subject = "/MqttNotice.Msg",
                Data = data
            }, DefalutNatsJsonSerializer<List<string>>.Default);
        }

        /// <summary>
        /// 更新协议缓存物模型信息
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public async Task<Dictionary<string, string>> DownUpdateProductSys(MZ_IotProduct product)
        {
            IotRedisHelper redis = _provider.GetService<IotRedisHelper>();
            Dictionary<string, string> sysdict = new Dictionary<string, string>();
            sysdict.Add("$Version", product.Version.ToString());
            if (!string.IsNullOrEmpty(product.ModelTSL))
            {
                sysdict.Add("$ModelTSL", product.ModelTSL);
            }
            if (!string.IsNullOrEmpty(product.StorageConfig))
            {
                sysdict.Add("$Storage", product.StorageConfig);
            }
            if (!string.IsNullOrEmpty(product.InterScripts))
            {
                sysdict.Add("$Script", product.InterScripts);
            }
            if (!string.IsNullOrEmpty(product.Status))
            {
                sysdict.Add("$Status", product.Status);
            }
            if (!string.IsNullOrEmpty(product.NetworkWay))
            {
                sysdict.Add("$NetworkWay", product.NetworkWay);
            }
            await redis.HashSetAsync("ProductSys:" + product.Id, sysdict);
            await _provider.GetService<ServerBusProxy>().PublishKeyDel("ProductSys:" + product.Id);
            return sysdict;
        }

        private async Task<T> WaitDown<I, T>(I msg) where I : BaseDeviceMessage where T : BaseUpDeviceMessage
        {
            try
            {
                var bus = _provider.GetService<NatsScope>().Bus;
                var requestTimeout = TimeSpan.FromSeconds(8);
                await using var resSub = await bus.SubscribeCoreAsync<T>(msg.MessageId, null, DefalutNatsJsonSerializer<T>.Default, new NatsSubOpts
                {
                    MaxMsgs = 1,
                    Timeout = requestTimeout,
                    StartUpTimeout = requestTimeout,
                    ThrowIfNoResponders = true
                });


                string msgbody = System.Text.Json.JsonSerializer.Serialize(msg, JsonMessageSerializerConfig.DefaultOptions);
                await bus.PublishAsync(GetDownKey(msg.DeviceId), msgbody, null, msg.MessageId, DefalutNatsJsonSerializer<string>.Default);

                await foreach (var responseMsg in resSub.Msgs.ReadAllAsync())
                {
                    return responseMsg.Data;
                }
                throw new TimeoutException($"等待 {requestTimeout.TotalSeconds} 秒后未收到回复");

            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
                return null;
            }
        }


        /// <summary>
        /// 读取指定设备的属性
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="deviceId"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        public async Task<BusResponse<Dictionary<string, DevicePropertyValue>>> WaitDownReadProperty(string productId, string deviceId, List<string> properties)
        {
            properties.Sort();
            ReadPropertyMessage msg = new ReadPropertyMessage();
            msg.DeviceId = deviceId;
            msg.ProductId = productId;
            msg.Properties = properties;
            msg.MessageId = $"Rd{deviceId}-{properties.Count}-{UtilityTool.MD5(string.Join('#', properties))}";
            var rs = await WaitDown<ReadPropertyMessage, ReadPropertyMessageReply>(msg);
            if (rs == null)
            {
                return BusResponse<Dictionary<string, DevicePropertyValue>>.Error(119, "读取指定属性超时");
            }
            return BusResponse<Dictionary<string, DevicePropertyValue>>.Success(DevicePropertyValue.FromDict(rs.Properties, MyAccess.Core.TypeConvert.Unix2Time(rs.Timestamp)));
        }
        public async Task DownReadProperty(string productId, string deviceId, List<string> properties)
        {
            properties.Sort();
            ReadPropertyMessage msg = new ReadPropertyMessage();
            msg.DeviceId = deviceId;
            msg.ProductId = productId;
            msg.Properties = properties;
            msg.MessageId = string.Empty;
            var bus = _provider.GetService<NatsScope>().Bus;
            string msgbody = System.Text.Json.JsonSerializer.Serialize(msg, JsonMessageSerializerConfig.DefaultOptions);

            await bus.PublishAsync(new NatsMsg<string>()
            {
                Subject = GetDownKey(msg.DeviceId),
                Data = msgbody
            }, DefalutNatsJsonSerializer<string>.Default);
        }

        public async Task ConfirmPropertyReply(ReadPropertyMessageReply msg)
        {
            var proplist = msg.Properties.Select(x => x.Key).ToList();
            proplist.Sort();
            string msgId = $"Rd{msg.DeviceId}-{proplist.Count}-{UtilityTool.MD5(string.Join('#', proplist))}";
            var bus = _provider.GetService<NatsScope>().Bus;
            string msgbody = System.Text.Json.JsonSerializer.Serialize(msg, JsonMessageSerializerConfig.DefaultOptions);
            await bus.PublishAsync(new NatsMsg<string>()
            {
                Subject = msgId,
                Data = msgbody
            }, DefalutNatsJsonSerializer<string>.Default);
        }


        /// <summary>
        /// 执行指定功能
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="deviceId"></param>
        /// <param name="functionId"></param>
        /// <param name="inputs"></param>
        /// <returns></returns>
        public async Task<BusResponse<IDictionary<string, object>>> DownFunction(string productId, string deviceId, string functionId, IDictionary<string, object> inputs)
        {
            FunctionInvokeMessage msg = new FunctionInvokeMessage();
            msg.DeviceId = deviceId;
            msg.ProductId = productId;
            msg.FunctionId = functionId;
            msg.Inputs = inputs;
            msg.MessageId = MyAccess.Core.StringTool.GetGUID();
            var rs = await WaitDown<FunctionInvokeMessage, FunctionInvokeMessageReply>(msg);
            if (rs == null)
            {
                return BusResponse<IDictionary<string, object>>.Error(119, "执行功能超时");
            }
            if (!rs.IsSuccess)
            {
                return BusResponse<IDictionary<string, object>>.Error(211, rs.Error);
            }
            return BusResponse<IDictionary<string, object>>.Success(rs.Outputs);
        }

        public async Task DownRawData(string productId, string deviceId, byte[] data)
        {
            RawDataMessage rawdata = new RawDataMessage();
            rawdata.Data = data;
            rawdata.DeviceId = deviceId;
            rawdata.MessageId = MyAccess.Core.StringTool.GetGUID();
            rawdata.ProductId = productId;

            var bus = _provider.GetService<NatsScope>().Bus;
            string msgbody = System.Text.Json.JsonSerializer.Serialize(rawdata, JsonMessageSerializerConfig.DefaultOptions);
            await bus.PublishAsync(new NatsMsg<string>()
            {
                Subject = GetDownKey(deviceId),
                Data = msgbody
            }, DefalutNatsJsonSerializer<string>.Default);
        }

        public async Task DownBind(string productId, string deviceId)
        {
            DeviceBindMessage msg = new DeviceBindMessage();
            msg.DeviceId = deviceId;
            msg.ProductId = productId;
            msg.MessageId = MyAccess.Core.StringTool.GetGUID();
            var bus = _provider.GetService<NatsScope>().Bus;
            string msgbody = System.Text.Json.JsonSerializer.Serialize(msg, JsonMessageSerializerConfig.DefaultOptions);
            await bus.PublishAsync(new NatsMsg<string>()
            {
                Subject = GetDownKey(msg.DeviceId),
                Data = msgbody
            }, DefalutNatsJsonSerializer<string>.Default);
        }

        public async Task DownICCID(string productId, string deviceId)
        {
            QueryICCIDMessage msg = new QueryICCIDMessage();
            msg.DeviceId = deviceId;
            msg.ProductId = productId;
            msg.MessageId = MyAccess.Core.StringTool.GetGUID();
            var bus = _provider.GetService<NatsScope>().Bus;
            string msgbody = System.Text.Json.JsonSerializer.Serialize(msg, JsonMessageSerializerConfig.DefaultOptions);
            await bus.PublishAsync(new NatsMsg<string>()
            {
                Subject = GetDownKey(msg.DeviceId),
                Data = msgbody
            }, DefalutNatsJsonSerializer<string>.Default);
        }
        /// <summary>
        /// 更新设备信息（发送设备绑定消息）
        /// </summary>
        /// <param name="device"></param>
        /// <param name="targetVersion"></param>
        /// <param name="channelConfig"></param>
        /// <returns></returns>
        public async Task<BusResponse<string>> DownSyncDevice(MZ_IotDevice device, int targetVersion, ChannelConfig channelConfig)
        {
            bool issuccess = true;
            string reason = string.Empty;

            if (channelConfig.CanBind && device.ProductVer < targetVersion)
            {
                //判断设备ProductVer，小于协议版本时，发送设备绑定
                DeviceBindMessage msg = new DeviceBindMessage();
                msg.DeviceId = device.DeviceId;
                msg.ProductId = device.ProductId;
                msg.MessageId = MyAccess.Core.StringTool.GetGUID();

                var reply = await WaitDown<DeviceBindMessage, DeviceBindMessageReply>(msg);
                if (reply == null)
                {
                    return BusResponse<string>.Error(119, "设备绑定超时");
                }
                issuccess = reply.IsSuccess;
                reason = reply.Reason;
            }

            if (issuccess)
            {
                //更新设备版本
                MZ_IotDevice newdevice = new MZ_IotDevice();
                newdevice.ProductVer = targetVersion;
                newdevice.Id = device.Id;
                await _provider.GetService<IotDeviceDAL>().Update(newdevice);
                return BusResponse<string>.Success();
            }
            else
            {
                return BusResponse<string>.Error(118, reason);
            }
        }

        /// <summary>
        /// 下发Modbus规则消息
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="deviceId"></param>
        /// <param name="matchName"></param>
        /// <returns></returns>
        public async Task DownModbusMessage(string productId, string deviceId, string matchName)
        {
            ModbusMessage msg = new ModbusMessage();
            msg.DeviceId = deviceId;
            msg.ProductId = productId;
            msg.MatchName = matchName;
            var bus = _provider.GetService<NatsScope>().Bus;
            string msgbody = System.Text.Json.JsonSerializer.Serialize(msg, JsonMessageSerializerConfig.DefaultOptions);

            await bus.PublishAsync(new NatsMsg<string>()
            {
                Subject = GetDownKey(msg.DeviceId),
                Data = msgbody
            }, DefalutNatsJsonSerializer<string>.Default);
        }

        private List<string> _upList;
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        /// <summary>
        /// 注册新的规则处理节点
        /// </summary>
        public void RegNode()
        {
            var option = _provider.GetService<IOptions<IotOption>>();
            IotRedisHelper redis = _provider.GetService<IotRedisHelper>();
            if (!string.IsNullOrEmpty(option.Value.node_name))
            {
                redis.HashSet("RuleExeNodes", option.Value.node_name, DateTime.Now.AddSeconds(600).ToString("o"));
            }
            var bus = _provider.GetService<NatsScope>().Bus;

            var t = bus.PublishAsync(new NatsMsg<string>()
            {
                Subject = "/RuleNode.Change",
                Data = string.Empty
            }, DefalutNatsJsonSerializer<string>.Default);
        }
        /// <summary>
        /// 注册新的规则处理节点（指定名称）
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task RegName(string name)
        {
            IotRedisHelper redis = _provider.GetService<IotRedisHelper>();
            await redis.HashSetAsync("RuleExeNodes", name, DateTime.Now.AddSeconds(600).ToString("o"));

            var bus = _provider.GetService<NatsScope>().Bus;

            await bus.PublishAsync(new NatsMsg<string>()
            {
                Subject = "/RuleNode.Change",
                Data = string.Empty
            }, DefalutNatsJsonSerializer<string>.Default);
        }
        /// <summary>
        /// 发送规则处理节点的心跳包
        /// </summary>
        /// <param name="nodename"></param>
        /// <returns></returns>
        public async Task TestUpNode(string nodename)
        {
            var bus = _provider.GetService<NatsScope>().Bus;
            await bus.PublishAsync(new NatsMsg<string>()
            {
                Subject = "/device.up." + nodename,
                Data = string.Empty
            }, DefalutNatsJsonSerializer<string>.Default);
        }
        /// <summary>
        /// 强制下线规则处理节点
        /// </summary>
        /// <param name="nodename"></param>
        /// <returns></returns>
        public async Task ForcedDownNode(string nodename)
        {
            IotRedisHelper redis = _provider.GetService<IotRedisHelper>();
            await redis.HashDeleteAsync("RuleExeNodes", nodename);

            var bus = _provider.GetService<NatsScope>().Bus;
            await bus.PublishAsync(new NatsMsg<string>()
            {
                Subject = "/RuleNode.Change",
                Data = string.Empty
            }, DefalutNatsJsonSerializer<string>.Default);
        }
        public void UpdateUpList()
        {
            IotRedisHelper redis = _provider.GetService<IotRedisHelper>();
            var dict = redis.HashGetAll<string>("RuleExeNodes");
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
        private string GetDownKey(string deviceId)
        {
            _lock.EnterReadLock();
            try
            {
                if (_upList == null || _upList.Count == 0) return "/device.dwn";
                int pos = Math.Abs(deviceId.GetHashCode() % _upList.Count);
                return "/device.dwn." + _upList[pos];
            }
            finally
            {
                _lock.ExitReadLock();
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
        public int GetIdx(string deviceId)
        {
            _lock.EnterReadLock();
            try
            {
                if (_upList == null || _upList.Count == 0) return 0;
                int pos = Math.Abs(deviceId.GetHashCode() % _upList.Count);
                return pos;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }
        public int GetNodeIdx()
        {
            var option = _provider.GetService<IOptions<IotOption>>();
            _lock.EnterReadLock();
            try
            {
                if (_upList == null || _upList.Count == 0) return 0;
                int idx = _upList.FindIndex(x => x == option.Value.node_name);
                if (idx == -1) return 0;
                return idx;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }
        public async Task StartReadAllMessage(string productId, string deviceId, List<string> props)
        {
            StartReadAllMessage msg = new StartReadAllMessage();
            msg.ProductId = productId;
            msg.DeviceId = deviceId;
            msg.Timestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
            msg.props = props;
            var bus = _provider.GetService<NatsScope>().Bus;
            await bus.PublishAsync(new NatsMsg<string>()
            {
                Subject = GetUpKey(deviceId),
                Data = System.Text.Json.JsonSerializer.Serialize(msg, JsonMessageSerializerConfig.DefaultOptions)
            }, DefalutNatsJsonSerializer<string>.Default);
        }


        /// <summary>
        /// 发送服务端事件
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="deviceId"></param>
        /// <param name="eventId"></param>
        /// <param name="outputs"></param>
        /// <param name="redirectFromProductId"></param>
        /// <param name="ruleId"></param>
        /// <param name="fromDtuId"></param>
        /// <returns></returns>
        public async Task SendEvent(string productId, string deviceId, string eventId, IDictionary<string, object> outputs, string redirectFromProductId = null, HashSet<long> ruleId = null, string fromDtuId = null, string fromNode = null)
        {
            DeviceEventMessage msg = new DeviceEventMessage();
            msg.DeviceId = deviceId;
            msg.ProductId = productId;
            msg.EventId = eventId;
            msg.Outputs = outputs;
            msg.Timestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
            msg.RedirectFromProductId = redirectFromProductId;
            msg.RuleIds = ruleId;
            msg.RedirecDtuId = fromDtuId;
            msg.NodeGuid = fromNode;
            var bus = _provider.GetService<NatsScope>().Bus;
            await bus.PublishAsync(new NatsMsg<string>()
            {
                Subject = GetUpKey(deviceId),
                Data = System.Text.Json.JsonSerializer.Serialize(msg, JsonMessageSerializerConfig.DefaultOptions)
            }, DefalutNatsJsonSerializer<string>.Default);
        }

        /// <summary>
        /// 发送服务端上线事件
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="deviceId"></param>
        /// <param name="ip"></param>
        /// <param name="redirectFromProductId"></param>
        /// <returns></returns>
        public async Task SendConnect(string productId, string deviceId, string ip = "", string redirectFromProductId = null, HashSet<long> ruleId = null, string fromDtuId = null, string fromNode = null)
        {
            DeviceOnlineMessage msg = new DeviceOnlineMessage();
            msg.ProductId = productId;
            msg.DeviceId = deviceId;
            msg.Timestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
            msg.RedirectFromProductId = redirectFromProductId;
            msg.RuleIds = ruleId;
            msg.RedirecDtuId = fromDtuId;
            msg.NodeGuid = fromNode;
            if (string.IsNullOrEmpty(ip))
            {
                msg.IpAddress = ip;
            }
            var bus = _provider.GetService<NatsScope>().Bus;
            await bus.PublishAsync(new NatsMsg<string>()
            {
                Subject = GetUpKey(deviceId),
                Data = System.Text.Json.JsonSerializer.Serialize(msg, JsonMessageSerializerConfig.DefaultOptions)
            }, DefalutNatsJsonSerializer<string>.Default);
        }


        /// <summary>
        /// 发送服务端离线事件
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="deviceId"></param>
        /// <param name="redirectFromProductId"></param>
        /// <param name="ruleId"></param>
        /// <returns></returns>
        public async Task SendDisconnect(string productId, string deviceId, string redirectFromProductId = null, HashSet<long> ruleId = null, string fromDtuId = null)
        {
            DeviceOfflineMessage msg = new DeviceOfflineMessage();
            msg.ProductId = productId;
            msg.DeviceId = deviceId;
            msg.Timestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
            msg.RedirectFromProductId = redirectFromProductId;
            msg.RuleIds = ruleId;
            msg.RedirecDtuId = fromDtuId;
            var bus = _provider.GetService<NatsScope>().Bus;
            await bus.PublishAsync(new NatsMsg<string>()
            {
                Subject = GetUpKey(deviceId),
                Data = System.Text.Json.JsonSerializer.Serialize(msg, JsonMessageSerializerConfig.DefaultOptions)
            }, DefalutNatsJsonSerializer<string>.Default);
        }






        /// <summary>
        /// 发送服务端属性回复
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="deviceId"></param>
        /// <param name="properties"></param>
        /// <param name="redirectFromProductId"></param>
        /// <param name="isTagSync"></param>
        /// <param name="ruleId"></param>
        /// <param name="fromDtuId"></param>
        /// <param name="indate"></param>
        /// <param name="fromNode"></param>
        /// <returns></returns>
        public async Task SendPropertyReply(string productId, string deviceId, IDictionary<string, object> properties, string redirectFromProductId = null, bool isTagSync = false, HashSet<long> ruleId = null, string fromDtuId = null, DateTime? indate = null, string fromNode = null)
        {
            ReadPropertyMessageReply msg = new ReadPropertyMessageReply();
            msg.ProductId = productId;
            msg.DeviceId = deviceId;
            if (indate == null)
            {
                msg.Timestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
            }
            else
            {
                msg.Timestamp = new DateTimeOffset(indate.Value).ToUnixTimeMilliseconds();
            }
            msg.Properties = properties;
            msg.RedirectFromProductId = redirectFromProductId;
            msg.IsTagSync = isTagSync;
            msg.RuleIds = ruleId;
            msg.RedirecDtuId = fromDtuId;
            msg.NodeGuid = fromNode;
            var bus = _provider.GetService<NatsScope>().Bus;
            await bus.PublishAsync(new NatsMsg<string>()
            {
                Subject = GetUpKey(deviceId),
                Data = System.Text.Json.JsonSerializer.Serialize(msg, JsonMessageSerializerConfig.DefaultOptions)
            }, DefalutNatsJsonSerializer<string>.Default);
        }
    }
}
