using FlowService.FlowNode.Builder;
using System;
namespace FlowService.FlowNode.Conditions
{
    public class DateCondition : BaseCondition
    {
        public string[] value { get; set; }
        public string compare { get; set; }
        public override string ToExpressionString(BuilderContext context)
        {
            string val = context.GetFormValue(this.id);
            if (this.compare == "before")
            {
                if (val == null)
                {
                    return "data.CompareDate(\"" + val + "\",\"" + value[0] + "\")<=0";
                }
                else
                {
                    DateTime dt1 = Convert.ToDateTime(val);
                    DateTime dt2 = Convert.ToDateTime(value[0]);
                    return dt1 <= dt2 ? "true" : "false";
                }
            }
            else
            {
                if (val == null)
                {
                    return "data.CompareDate(\"" + val + "\",\"" + value[0] + "\")>=0";
                }
                else
                {
                    DateTime dt1 = Convert.ToDateTime(val);
                    DateTime dt2 = Convert.ToDateTime(value[0]);
                    return dt1 >= dt2 ? "true" : "false";
                }
            }
        }
    }
}
