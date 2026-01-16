using ChannelUtility;
using ChannelUtility.Message;
using Common.EventBus;
using Common.Share;
using FlowService.Model;
using Microsoft.Extensions.Logging;
using NATS.Client.Core;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace FlowService
{
    public class DeviceBusProxy
    {
        private ITAServiceProvider _provider;
        private List<string> _upList;
        private ILogger<DeviceBusProxy> _log;
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        public DeviceBusProxy(ITAServiceProvider provider, ILoggerFactory logFactory)
        {
            _provider = provider;
            _log = logFactory.CreateLogger<DeviceBusProxy>();
        }
        public void UpdateUpList()
        {
            DeviceRedisHelper redis = _provider.GetService<DeviceRedisHelper>();
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
        private string GetDownKey(string deviceId)
        {
            _lock.EnterReadLock();
            try
            {
                if (_upList == null || _upList.Count == 0) return "device.dwn";
                int pos = Math.Abs(deviceId.GetHashCode() % _upList.Count);
                return "device.dwn." + _upList[pos];
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
        /// <returns></returns>
        public async Task SendEvent(string productId, string deviceId, string eventId, IDictionary<string, object> outputs)
        {
            DeviceEventMessage msg = new DeviceEventMessage();
            msg.DeviceId = deviceId;
            msg.ProductId = productId;
            msg.EventId = eventId;
            msg.Outputs = outputs;
            msg.Timestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
            var bus = _provider.GetService<NatsScope>().Bus;

            await bus.PublishAsync(new NatsMsg<string>()
            {
                Subject = GetUpKey(deviceId),
                Data = System.Text.Json.JsonSerializer.Serialize(msg, JsonMessageSerializerConfig.DefaultOptions)
            }, DefalutNatsJsonSerializer<string>.Default);
        }

        private async Task<FunctionInvokeMessageReply> WaitDown(Out_FlowDevice device, FunctionInvokeMessage msg)
        {
            try
            {
                var bus = _provider.GetService<NatsScope>().Bus;
                var requestTimeout = TimeSpan.FromSeconds(8);
                await using var resSub = await bus.SubscribeCoreAsync(msg.MessageId, null, DefalutNatsJsonSerializer<FunctionInvokeMessageReply>.Default, new NatsSubOpts
                {
                    MaxMsgs = 1,
                    Timeout = requestTimeout,
                    ThrowIfNoResponders = true
                }).ConfigureAwait(false);


                string msgbody = System.Text.Json.JsonSerializer.Serialize(msg, JsonMessageSerializerConfig.DefaultOptions);
                await bus.PublishAsync(GetDownKey(msg.DeviceId), msgbody, null, msg.MessageId, DefalutNatsJsonSerializer<string>.Default).ConfigureAwait(false);

                await foreach (var responseMsg in resSub.Msgs.ReadAllAsync().ConfigureAwait(false))
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
        /// 执行指定功能
        /// </summary>
        /// <param name="context"></param>
        /// <param name="device"></param>
        /// <param name="functionId"></param>
        /// <param name="inputs"></param>
        /// <returns></returns>
        public async Task<BusResponse<IDictionary<string, object>>> DownFunction(Out_FlowDevice device, string functionId, IDictionary<string, object> inputs)
        {
            FunctionInvokeMessage msg = new FunctionInvokeMessage();
            msg.DeviceId = device.DeviceId;
            msg.ProductId = device.ProductId;
            msg.FunctionId = functionId;
            msg.Inputs = inputs;
            msg.MessageId = MyAccess.Core.StringTool.GetGUID();
            var rs = await WaitDown(device, msg);
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

    }
}
