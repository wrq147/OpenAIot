using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTVideoService
{
    public class VideoOption
    {
        public string minio_url { get; set; }
        public string minio_bucket { get; set; }
        public List<ServerInfo> VideoServers { get; set; }
        public List<ServerInfo> GB28181Servers { get; set; }
    }

    public class ServerInfo
    {
        public string NodeId { get; set; }
        public string Ip { get; set; }
        public int RtmpPort { get; set; }
        public int HttpPort { get; set; }
    }
}
