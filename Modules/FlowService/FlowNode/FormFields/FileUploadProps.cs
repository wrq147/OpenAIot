using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowService.FlowNode.FormFields
{
    public class FileUploadProps : BaseProps
    {
        public string placeholder { get; set; }
        public int maxSize { get; set; }
        public int maxNumber { get; set; }
        public string[] fileTypes { get; set; }
    }
}
