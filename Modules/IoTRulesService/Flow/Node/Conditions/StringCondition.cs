using IoTRulesService.Flow.Builder;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Common;

namespace IoTRulesService.Flow.Node.Conditions
{
    public class StringCondition : BaseCondition
    {
        public string value { get; set; }
        public string compare { get; set; }
        /// <summary>
        /// 0为值、1为参数
        /// </summary>
        public int valueFrom { get; set; }
        public override async Task<bool> ToExpressionString(RuleExecutionContext context)
        {
            string compareval;
            if (valueFrom == 1)
            {
                compareval = TAConverter.Cast<string>(context.GetParam(value));
            }
            else
            {
                compareval = TAConverter.Cast<string>(value);
            }
            if (compareval != null)
            {
                compareval = compareval.Trim();
            }
            var tval = await context.ReadSourceString(this.code);
            if (tval == null)
            {
                return false;
            }
            if (compare == "=")
            {
                return tval == compareval;
            }
            else if (compare == "!=")
            {
                return tval != compareval;
            }
            else if (compare == "IN")
            {
                var comvv = (await context.ReadSourceString(this.code));
                if (comvv != null)
                {
                    return comvv.IndexOf(compareval) >= 0;
                }
            }
            return false;
        }
    }
}
