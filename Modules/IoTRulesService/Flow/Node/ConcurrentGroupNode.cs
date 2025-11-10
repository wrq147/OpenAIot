using System;

namespace IoTRulesService.Flow.Node
{
    public class ConcurrentGroupNode : RuleBaseNode
    {
        public ConcurrentNode[] branchs { get; set; }
    }
}
