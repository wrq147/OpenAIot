using IoTRulesService.Flow.Node;
using System;
using System.Threading.Tasks;

namespace IoTRulesService.Flow.Builder.Step
{
    public class IfStep : RuleflowStep
    {
        public ConditionsProps props { get; set; }
        public int FirstIndex { get; set; }
        /// <summary>
        /// 是否为最后的条件
        /// </summary>
        public bool IsLastCondition { get; set; }
        /// <summary>
        /// 条件是否已激活过
        /// </summary>
        public bool Result { get; set; }
        public override async Task Run(RuleExecutionContext context)
        {
            if (this.Index == FirstIndex)
            {
                //首个条件先清标记
                Result = false;
            }
            else
            {
                IfStep first = context.Steps[FirstIndex] as IfStep;
                if (first.Result)
                {
                    await context.ExcuteNext(RuleResult.Next());
                    return;
                }
            }

            bool exrs = await this.props.ToExpressionString(context);
            if (context.IsDebug)
            {
                await context.Print("条件结果:" + (exrs ? "真" : "假"));
            }
            if (exrs)
            {
                IfStep first = context.Steps[FirstIndex] as IfStep;
                first.Result = true;
                var res = new RuleResult();
                res.ActiveChildren = true;
                await context.ExcuteNext(res);
                return;
            }
            else
            {
                if (!this.IsLastCondition)
                {
                    await context.ExcuteNext(RuleResult.Next());
                }
            }

        }
    }
}
