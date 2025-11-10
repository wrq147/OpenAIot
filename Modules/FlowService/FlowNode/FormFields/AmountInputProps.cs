using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowService.FlowNode.FormFields
{
    public class AmountInputProps : BaseProps
    {
        public string placeholder { get; set; }
        public bool showChinese { get; set; }
        public double precision { get; set; }
    }
}
