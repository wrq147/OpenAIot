using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowService.FlowNode.Builder
{
    public class NodeLinkItem
    {
        public NodeLinkItem(string t, string id)
        {
            this.type = t;
            this.id = id;
        }
        public string type { get; set; }
        public string id { get; set; }
    }
}
