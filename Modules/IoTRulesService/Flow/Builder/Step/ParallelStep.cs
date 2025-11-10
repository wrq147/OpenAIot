using System;
using System.Threading.Tasks;

namespace IoTRulesService.Flow.Builder.Step
{
    /// <summary>
    /// 并行节点
    /// </summary>
    public class ParallelStep : RuleflowStep
    {
        public int EndCount = 0;
        public override async Task Run(RuleExecutionContext context)
        {
            EndCount = 0;
            var res = new RuleResult();
            res.Directive = RuleResultDirective.Next;
            res.ActiveChildren = true;
            res.Parallel = true;

            await context.ExcuteNext(res);
        }
    }
}
