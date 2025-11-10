using System;
using System.Threading.Tasks;

namespace IoTRulesService.Flow.Builder.Step
{
    /// <summary>
    /// 并行结束
    /// </summary>
    public class ParallelEnd : RuleflowStep
    {
        public override async Task Run(RuleExecutionContext context)
        {
            if (this.IsActive)
            {
                return;
            }
            this.IsActive = true;
            var parentStep = context.Steps[context.Steps[this.ParentIndex].ParentIndex] as ParallelStep;
            ++parentStep.EndCount;
            if (parentStep.EndCount == parentStep.Children.Count)
            {
                for (int i = this.Index; i < context.Steps.Count; i++)
                {
                    if (context.Steps[i].Level <= context.Steps[this.ParentIndex].Level)
                    {
                        context.ExcuteIndex = i - 1;
                        break;
                    }
                }
                await context.ExcuteNext(RuleResult.Next());
            }
        }
    }
}
