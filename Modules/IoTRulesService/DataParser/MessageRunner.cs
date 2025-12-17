using ChannelUtility;
using ChannelUtility.Message;
using IoTService;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace IoTRulesService.DataParser
{
    public class MessageRunner
    {
        private ITAServiceProvider _provider;
        public delegate Task BaseMessageHandler(BaseDeviceMessage msg);
        public event BaseMessageHandler OtherMessageListener;
        public MessageRunner(ITAServiceProvider provider)
        {
            _provider = provider;
        }
        /// <summary>
        /// 单线程处理所有的消息
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>

        public async Task ParseExe(string msg)
        {
            if (string.IsNullOrEmpty(msg))
            {
                var tmpoption = _provider.GetService<IOptions<IotOption>>();
                var redis = _provider.GetService<IotRedisHelper>();
                await redis.HashSetAsync("RuleExeNodes", tmpoption.Value.node_name, DateTime.Now.AddSeconds(130).ToString("o"));
                return;
            }

            var rs = System.Text.Json.JsonSerializer.Deserialize<BaseDeviceMessage>(msg, JsonMessageSerializerConfig.DefaultOptions);
            if (rs is RawUpDataMessage rawUpData)
            {
                await _provider.GetService<DeviceMessageHandler>().ParseMessage(rawUpData);
            }
            else if (rs is BaseUpDeviceMessage upMsg)
            {
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
    }
}
