using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowService.FlowNode.FormFields
{
    public class SelectInputProps : BaseProps
    {
        public string placeholder { get; set; }
        public bool expanding { get; set; }
        public string[] options { get; set; }
    }
}
