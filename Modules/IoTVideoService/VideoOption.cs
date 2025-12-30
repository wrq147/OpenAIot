using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTVideoService
{
    public class VideoOption
    {
        public List<FixServer> VideoServers { get; set; }
        public List<GB28181Server> GB28181Servers { get; set; }
    }
    public class FixServer
    {
        public string Ip { get; set; }
        public int Port { get; set; }
    }
    public class GB28181Server
    {
        public string Node { get; set; }
        public string Ip { get; set; }
        public int Port { get; set; }
    }
}
