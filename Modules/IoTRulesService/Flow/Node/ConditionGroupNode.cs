using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTRulesService.Flow.Node
{
    public class ConditionGroupNode : RuleBaseNode
    {
        public object props { get; set; }
        public ConditionNode[] branchs { get; set; }
    }

}
