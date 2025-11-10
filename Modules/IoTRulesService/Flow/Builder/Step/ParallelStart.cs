using System;
using System.Threading.Tasks;
namespace IoTRulesService.Flow.Builder.Step
{
    public class ParallelStart : RuleflowStep
    {

        public override async Task Run(RuleExecutionContext context)
        {
            if (context.IsDebug)
            {
                await context.Print("并行输入:" + Newtonsoft.Json.JsonConvert.SerializeObject(context.Data));
            }
            var res = new RuleResult();
            res.ActiveChildren = true;
            await context.ExcuteNext(res);

            //执行End
            var endStep = context.Steps[this.Children[this.Children.Count - 1]] as ParallelEnd;
            context.ExcuteIndex = endStep.Index - 1;
            await context.ExcuteNext(RuleResult.Next());
        }
    }
}
