using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageService.Model
{
    public class SinglePushResponse
    {
        public int code { get; set; }
        public string msg { get; set; }
        public object data { get; set; }
    }
}
