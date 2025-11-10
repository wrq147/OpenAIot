using System;
namespace IoTRulesService.Flow.Node
{
    public class ClearDeltaNode : RuleBaseNode
    {
        public ClearProps props { get; set; }
    }
    public class ClearProps
    {
        public int ClearType { get; set; }
        public string[] Codes { get; set; }
    }
}
