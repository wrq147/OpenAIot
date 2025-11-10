using AuthService;
using FlowService.FlowNode.Builder;
using FlowService.Model;
using System;
using System.Collections.Generic;

namespace FlowService.FlowNode.Conditions
{
    public class DeptCondition : BaseCondition
    {
        public ObjData[] value { get; set; }
        public override string ToExpressionString(BuilderContext context)
        {
            return "false";
        }
    }
}
