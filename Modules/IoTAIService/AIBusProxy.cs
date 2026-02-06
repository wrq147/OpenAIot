using ChannelUtility;
using ChannelUtility.Message;
using ChannelUtility.Redis;
using Common.EventBus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NATS.Client.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace IoTAIService
{
    public class AIBusProxy
    {
        private ITAServiceProvider _provider;
        private ILogger<AIBusProxy> _log;
        public AIBusProxy(ITAServiceProvider provider, ILoggerFactory logFactory)
        {
            _provider = provider;
            _log = logFactory.CreateLogger<AIBusProxy>();
        }
        public async Task RegNode()
        {
            var option = _provider.GetService<IOptions<IoTAIOption>>();
            GeneralRedisHelper redis = _provider.GetService<GeneralRedisHelper>();
            if (!string.IsNullOrEmpty(option.Value.AINodeName))
            {
                await redis.HashSetAsync("AIExeNodes", option.Value.AINodeName, DateTime.Now.AddSeconds(600).ToString("o"));
            }
            var bus = _provider.GetService<NatsScope>().Bus;

            await bus.PublishAsync(new NatsMsg<string>()
            {
                Subject = "AINode.Change",
                Data = string.Empty
            }, DefalutNatsJsonSerializer<string>.Default);
        }
        private List<string> _upList;
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        public async Task UpdateUpList()
        {
            GeneralRedisHelper redis = _provider.GetService<GeneralRedisHelper>();
            var dict = await redis.HashGetAllAsync<string>("RuleExeNodes");
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
            msg.NodeId = fromNode;
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
        /// <returns></returns>
        public async Task SendPropertyReply(string productId, string deviceId, IDictionary<string, object> properties)
        {
            ReadPropertyMessageReply msg = new ReadPropertyMessageReply();
            msg.ProductId = productId;
            msg.DeviceId = deviceId;
            msg.Timestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
            msg.Properties = properties;
            msg.RedirectFromProductId = null;
            msg.IsTagSync = false;
            msg.RuleIds = null;
            msg.RedirecDtuId = null;
            msg.NodeId = null;
            var bus = _provider.GetService<NatsScope>().Bus;
            await bus.PublishAsync(new NatsMsg<string>()
            {
                Subject = GetUpKey(deviceId),
                Data = System.Text.Json.JsonSerializer.Serialize(msg, JsonMessageSerializerConfig.DefaultOptions)
            }, DefalutNatsJsonSerializer<string>.Default);
        }
    }
}
