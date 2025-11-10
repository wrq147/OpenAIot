using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MqttService
{
    public class ConnectPayload
    {
        public string username { get; set; }
        public long ts { get; set; }
        public int sockport { get; set; }
        public int proto_ver { get; set; }
        public string proto_name { get; set; }
        public int keepalive { get; set; }
        public string ipaddress { get; set; }
        public int expiry_interval { get; set; }
        public long connected_at { get; set; }
        public int connack { get; set; }
        public string clientid { get; set; }
        public bool clean_start { get; set; }
    }
}
