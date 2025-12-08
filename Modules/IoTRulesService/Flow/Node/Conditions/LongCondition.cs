using IoTRulesService.Flow.Builder;
using System.Threading.Tasks;
using TemplateAction.Common;

namespace IoTRulesService.Flow.Node.Conditions
{
    public class LongCondition : BaseCondition
    {
        public string value { get; set; }
        public string compare { get; set; }
        /// <summary>
        /// 0为值、1为参数
        /// </summary>
        public int valueFrom { get; set; }
        public override async Task<bool> ToExpressionString(RuleExecutionContext context)
        {
            long? compareval;
            if (valueFrom == 1)
            {
                compareval = await context.ReadSourceLong(value);
            }
            else
            {
                compareval = TAConverter.Cast<long>(value);
            }
            var tval = await context.ReadSourceLong(this.code);
            if (tval == null)
            {
                return false;
            }
            switch (compare)
            {
                case "=":
                    return tval == compareval;
                case "!=":
                    return tval != compareval;
                case ">":
                    return tval > compareval;
                case ">=":
                    return tval >= compareval;
                case "<":
                    return tval < compareval;
                case "<=":
                    return tval <= compareval;
                default:
                    return false;
            }
        }
    }
}
