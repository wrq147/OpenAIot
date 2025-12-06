using IoTRulesService.Flow.Builder;
using System.Threading.Tasks;
using TemplateAction.Common;

namespace IoTRulesService.Flow.Node.Conditions
{
    public class BoolCondition : BaseCondition
    {
        public string value { get; set; }
        public string compare { get; set; }
        /// <summary>
        /// 0为值、1为参数
        /// </summary>
        public int valueFrom { get; set; }
        public override async Task<bool> ToExpressionString(RuleExecutionContext context)
        {
            bool? compareval;
            if (valueFrom == 1)
            {
                compareval = await context.ReadSourceBool(value);
            }
            else
            {
                compareval = TAConverter.Cast<bool>(value);
            }
            if (this.compare == "=")
            {
                return (await context.ReadSourceBool(code)) == compareval;
            }
            else
            {
                return (await context.ReadSourceBool(code)) != compareval;
            }
        }
    }
}
