using ChannelUtility;
using ChannelUtility.Config;
using ChannelUtility.Message;
using Common.EventBus;
using Common.Share;
using EasyNetQ;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TemplateAction.Core;
using System.Linq;
using FlowService.FlowNode.Builder;
using FlowService.Model;

namespace FlowService
{
    public class DeviceBusProxy
    {
        private ITAServiceProvider _provider;
        private List<string> _upList;
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        public DeviceBusProxy(ITAServiceProvider provider)
        {
            _provider = provider;
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
        /// 发送服务端事件
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="deviceId"></param>
        /// <param name="eventId"></param>
        /// <param name="outputs"></param>
        /// <param name="redirectFromProductId"></param>
        /// <returns></returns>
        public async Task SendEvent(string productId, string deviceId, string eventId, IDictionary<string, object> outputs)
        {
            DeviceEventMessage msg = new DeviceEventMessage();
            msg.DeviceId = deviceId;
            msg.ProductId = productId;
            msg.EventId = eventId;
            msg.Outputs = outputs;
            msg.Timestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
            var bus = _provider.GetService<RabbitScope>().Bus;
            await bus.PubSub.PublishAsync(JsonConvert.SerializeObject(msg), GetUpKey(deviceId));
        }

        private async Task<FunctionInvokeMessageReply> WaitDown(Out_FlowDevice device, FunctionInvokeMessage msg)
        {
            var bus = _provider.GetService<RabbitScope>().Bus;
            var tcs = new TaskCompletionSource<FunctionInvokeMessageReply>(TaskCreationOptions.RunContinuationsAsynchronously);
            //8秒后自动取消
            var cts = new CancellationTokenSource(8000);
            cts.Token.Register(() => tcs.TrySetCanceled(), useSynchronizationContext: false);

            var rs = await bus.SendReceive.ReceiveAsync<FunctionInvokeMessageReply>("bus.response." + msg.MessageId, msg =>
            {
                tcs.TrySetResult(msg);
            }, cfg =>
            {
                cfg.WithAutoDelete(true);
            }, cts.Token);
            try
            {
                string msgbody = JsonConvert.SerializeObject(msg);
                await bus.PubSub.PublishAsync(msgbody, "/device." + device.NetworkWay + ".down");
                var reply = await tcs.Task.ConfigureAwait(false);
                rs.Dispose();
                return reply;
            }
            catch (Exception ex)
            {
                rs.Dispose();
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
