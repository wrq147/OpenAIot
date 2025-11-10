
using IoTRulesService.Flow.Builder;
using System;
using System.Threading.Tasks;
using TemplateAction.Common;

namespace IoTRulesService.Flow.Node.Conditions
{
    public class DateCondition : BaseCondition
    {
        public string value { get; set; }
        public string compare { get; set; }
        /// <summary>
        /// 0为值、1为参数
        /// </summary>
        public int valueFrom { get; set; }
        public override async Task<bool> ToExpressionString(RuleExecutionContext context)
        {
            long valtime;
            if (valueFrom == 1)
            {
                valtime = Convert.ToInt64(context.GetParam(value));
            }
            else
            {
                valtime = Convert.ToInt64(value);
            }
            var tval = await context.ReadSourceTime(code);
            if (tval == null) return false;
            if (this.compare == "before")
            {

                return tval <= valtime;
            }
            else
            {
                return tval >= valtime;
            }
        }
    }
}
