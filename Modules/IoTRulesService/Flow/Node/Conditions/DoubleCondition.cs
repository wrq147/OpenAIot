using IoTRulesService.Flow.Builder;
using System;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Common;

namespace IoTRulesService.Flow.Node.Conditions
{
    public class DoubleCondition : BaseCondition
    {
        public string value { get; set; }
        public string compare { get; set; }
        /// <summary>
        /// 0为值、1为参数
        /// </summary>
        public int valueFrom { get; set; }
        public override async Task<bool> ToExpressionString(RuleExecutionContext context)
        {
            double? compareval;
            if (valueFrom == 1)
            {
                compareval = await context.ReadSourceDouble(value);
            }
            else
            {
                compareval = TAConverter.Cast<double>(value);
            }
            var tval = await context.ReadSourceDouble(this.code);
            if (tval == null) return false;
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
