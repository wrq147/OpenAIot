using System;

namespace FlowService.FlowNode.Builder.Step
{
    public class OptionItem
    {
        public OptionItem(string a,string t)
        {
            this.action = a;
            this.type = t;
        }
        public string action { get; set; }
        public string type { get; set; }
    }
}
