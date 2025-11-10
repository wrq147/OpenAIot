using AuthService;
using FlowService.FlowNode.Builder;
using System.Linq;
using MyAccess.Core;
using System;
namespace FlowService.FlowNode.Conditions
{
    public class NumberCondition : BaseCondition
    {
        public double[] value { get; set; }
        public string compare { get; set; }
        public override string ToExpressionString(BuilderContext context)
        {
            string val = context.GetFormValue(this.id);
            if (val == null)
            {
                switch (compare)
                {
                    case "=":
                        return "data.GetFormDouble(\"" + this.id + "\")==" + value[0];
                    case "!=":
                        return "data.GetFormDouble(\"" + this.id + "\")!=" + value[0];
                    case ">":
                        return "data.GetFormDouble(\"" + this.id + "\")>" + value[0];
                    case ">=":
                        return "data.GetFormDouble(\"" + this.id + "\")>=" + value[0];
                    case "<":
                        return "data.GetFormDouble(\"" + this.id + "\")<" + value[0];
                    case "<=":
                        return "data.GetFormDouble(\"" + this.id + "\")<=" + value[0];
                    case "B":
                        return value[0] + "<data.GetFormDouble(\"" + this.id + "\")&&data.GetFormDouble(\"" + this.id + "\")<" + value[1];
                    case "AB":
                        return value[0] + "<=data.GetFormDouble(\"" + this.id + "\")&&data.GetFormDouble(\"" + this.id + "\")<" + value[1];
                    case "BA":
                        return value[0] + "<data.GetFormDouble(\"" + this.id + "\")&&data.GetFormDouble(\"" + this.id + "\")<=" + value[1];
                    case "ABA":
                        return value[0] + "<=data.GetFormDouble(\"" + this.id + "\")&&data.GetFormDouble(\"" + this.id + "\")<=" + value[1];
                }
            }
            else
            {
                double dbval = Convert.ToDouble(val);
                switch (compare)
                {
                    case "=":
                        return dbval == value[0] ? "true" : "false";
                    case "!=":
                        return dbval != value[0] ? "true" : "false";
                    case ">":
                        return dbval > Convert.ToDouble(value[0]) ? "true" : "false";
                    case ">=":
                        return dbval >= value[0] ? "true" : "false";
                    case "<":
                        return dbval < value[0] ? "true" : "false";
                    case "<=":
                        return dbval <= value[0] ? "true" : "false";
                    case "B":
                        return (value[0] < dbval && dbval < value[1]) ? "true" : "false";
                    case "AB":
                        return (value[0] <= dbval && dbval < value[1]) ? "true" : "false";
                    case "BA":
                        return (value[0] < dbval && dbval <= value[1]) ? "true" : "false";
                    case "ABA":
                        return (value[0] <= dbval && dbval <= value[1]) ? "true" : "false";
                }
            }

            return "false";
        }
    }
}
