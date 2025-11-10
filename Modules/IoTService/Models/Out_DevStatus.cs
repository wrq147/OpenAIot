using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTService.Models
{
    public class Out_DevStatus
    {
        public int TotalCount { get; set; }
        public int OnlineCount { get; set; }
        public int OfflineCount { get; set; }
        public int UnknowCount { get; set; }
    }
}
