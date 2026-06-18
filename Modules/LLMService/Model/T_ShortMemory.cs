using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLMService.Model
{
    public class T_ShortMemory
    {
        public string User { get; set; }
        public string Tool { get; set; }
        public string Assistant { get; set; }
        public string Time { get; set; }
    }
}
