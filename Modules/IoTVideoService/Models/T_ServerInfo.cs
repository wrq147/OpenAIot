using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTVideoService.Models
{
    public class T_ServerInfo
    {
        public string NodeId { get; set; }
        public string Ip { get; set; }
        public int RtmpPort { get; set; }
        public int HttpPort { get; set; }
        public DateTime Expire { get; set; }
    }
}
