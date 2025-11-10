using FlowService.FlowNode.Conditions;
using System;
using System.Threading.Tasks;

namespace FlowService.FlowNode
{
    /// <summary>
    /// 条件组节点
    /// 包含多个条件节点
    /// </summary>
    public class ConditionGroupNode : FlowBaseNode
    {
        public object props { get; set; }
        public ConditionNode[] branchs { get; set; }
    }


}
