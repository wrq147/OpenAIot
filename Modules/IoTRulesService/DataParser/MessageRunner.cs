using ChannelUtility;
using ChannelUtility.Message;
using IoTService;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace IoTRulesService.DataParser
{
    public class MessageRunner
    {
        private ILogger<MessageRunner> _log;
        private ITAServiceProvider _provider;
        public delegate Task BaseMessageHandler(BaseDeviceMessage msg);
        public event BaseMessageHandler OtherMessageListener;
        public MessageRunner(ILoggerFactory factory, ITAServiceProvider provider)
        {
            _provider = provider;
            _log = factory.CreateLogger<MessageRunner>();
        }

        /// <summary>
        /// 单线程处理所有的消息
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="replyTo"></param>
        /// <returns></returns>

        public async Task ParseExe(string msg, string replyTo)
        {
            try
            {
                if (string.IsNullOrEmpty(msg))
                {
                    var tmpoption = _provider.GetService<IOptions<IotOption>>();
                    var redis = _provider.GetService<IotRedisHelper>();
                    await redis.HashSetAsync("RuleExeNodes", tmpoption.Value.node_name, DateTime.Now.AddSeconds(130).ToString("o"));
                    return;
                }

                var rs = System.Text.Json.JsonSerializer.Deserialize<BaseDeviceMessage>(msg, JsonMessageSerializerConfig.DefaultOptions);
                if (string.IsNullOrEmpty(rs.MessageId))
                {
                    rs.MessageId = replyTo;
                }
                if (rs is RawUpDataMessage rawUpData)
                {
                    if (!string.IsNullOrEmpty(rawUpData.NodeId))
                    {
                        _provider.GetService<PackParser>().UpdateDeviceGuid(rawUpData.DeviceId, rawUpData.NodeId);
                    }
                    await _provider.GetService<DeviceMessageHandler>().ParseMessage(rawUpData);
                }
                else if (rs is BaseUpDeviceMessage upMsg)
                {
                    if (!string.IsNullOrEmpty(upMsg.NodeId))
                    {
                        _provider.GetService<PackParser>().UpdateDeviceGuid(upMsg.DeviceId, upMsg.NodeId);
                    }
                    await _provider.GetService<DeviceMessageHandler>().ExeMessage(upMsg);
                }
                else
                {
                    if (OtherMessageListener != null)
                    {
                        await OtherMessageListener(rs);
                    }
                }
            }
            catch(Exception ex)
            {
                _log.LogError(ex.Message);
            }
        }
        public async Task ParseDown(BaseDeviceMessage msg)
        {
            await _provider.GetService<DeviceMessageHandler>().ParseDown(msg);
        }
        public async Task ParseDown(string msg)
        {
            try
            {
                var rs = System.Text.Json.JsonSerializer.Deserialize<BaseDeviceMessage>(msg, JsonMessageSerializerConfig.DefaultOptions);
                await this.ParseDown(rs);
            }
            catch(Exception ex)
            {
                _log.LogError(ex.Message);
            }
        }
    }
}
