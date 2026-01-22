using ChannelUtility.Tsl;
using Common.Json;
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
                await context.Print("输入数据:" + System.Text.Json.JsonSerializer.Serialize(context.Data, MyDefaultTextJsonConfig.DefaultOptions));
            }
            await context.ExcuteNext(RuleResult.Next());
        }
    }
}
