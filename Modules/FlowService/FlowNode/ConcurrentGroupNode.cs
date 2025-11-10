using System;

namespace FlowService.FlowNode
{
    /// <summary>
    /// 并行节点
    /// </summary>
    public class ConcurrentGroupNode : FlowBaseNode
    {
        public ConcurrentNode[] branchs { get; set; }
    }
}
