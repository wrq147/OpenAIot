using ChannelUtility.Message;
using IoTRulesService.Flow.Node;
using IoTService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace IoTRulesService.Flow.Builder.Step
{
    /// <summary>
    /// 报警节点
    /// </summary>
    public class WarnStep : RuleflowStep
    {
        public WarnProps props { get; set; }

        public override async Task Run(RuleExecutionContext context)
        {
            var dataItem = context.Data.LastOrDefault();
            IDictionary<string, object> inputs = null;
            if (dataItem != null)
            {
                inputs = dataItem.Data;
            }
            else
            {
                inputs = new Dictionary<string, object>();
            }

            string productId = context.Source.ProductId;
            string deviceId = context.Source.DeviceId;
            if (props.TargetType == 1)
            {
                var device = await context.GetDevice(props.TargetId);
                if (device == null)
                {
                    await context.Print("设备不存在");
                    return;
                }
                productId = device.ProductId;
                deviceId = device.DeviceId;
            }


            //如果为当前事件则不发送事件报警
            bool sendEvt = true;
            if (context.Source is DeviceEventMessage evtmsg)
            {
                if (props.EventId == evtmsg.EventId)
                {
                    sendEvt = false;
                }
            }


            if (sendEvt)
            {
                if (context.IsDebug)
                {
                    await context.Print("触发事件" + props.EventId + ",输入数据:" + Newtonsoft.Json.JsonConvert.SerializeObject(inputs));
                }
                await context.Provider.GetService<ServerBusProxy>().SendEvent(productId, deviceId, props.EventId, inputs, null, context.GetStartRuleId());
                await context.ExcuteNext(RuleResult.Next());
            }
            else
            {
                await context.Print("因事件循环触发,执行中断");
            }

        }

    }

}
