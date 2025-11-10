using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTRulesService.Flow.Builder.Step
{
    /// <summary>
    /// 终结流程
    /// </summary>
    public class EndStep : RuleflowStep
    {
        public override async Task Run(RuleExecutionContext context)
        {
            await context.ExcuteNext(RuleResult.End());
        }
    }
}
