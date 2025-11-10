using ChannelUtility.Tsl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IoTRulesService.Flow.Builder.Step
{
    /// <summary>
    /// 开始节点
    /// </summary>
    public class StartStep : RuleflowStep
    {
        public override async Task Run(RuleExecutionContext context)
        {
            if (context.IsDebug && this.Id == "root")
            {
                await context.Print("输入数据:" + Newtonsoft.Json.JsonConvert.SerializeObject(context.Data));
            }
            await context.ExcuteNext(RuleResult.Next());
        }
    }
}
