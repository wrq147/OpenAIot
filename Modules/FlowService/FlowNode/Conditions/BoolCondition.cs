using FlowService.FlowNode.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowService.FlowNode.Conditions
{
    public class BoolCondition : BaseCondition
    {
        public string[] value { get; set; }
        public string compare { get; set; }
        public override string ToExpressionString(BuilderContext context)
        {
            if (this.compare == "=")
            {
                return "data.FormEq(\"" + this.id + "\",\"" + value[0] + "\")";
            }
            else
            {
                return "!data.FormEq(\"" + this.id + "\",\"" + value[0] + "\")";
            }
        }
    }
}
