using System;
using System.Threading.Tasks;

namespace IoTRulesService.Flow.Builder.Step
{
    /// <summary>
    /// 什么都不做执行下一个节点
    /// </summary>
    public class NextStep : RuleflowStep
    {
        public override async Task Run(RuleExecutionContext context)
        {
            await context.ExcuteNext(RuleResult.Next());
        }
    }
}
