using System;
namespace MqttService
{
    public class DisconnectPayload
    {
        public string username { get; set; }
        public long ts { get; set; }
        public int sockport { get; set; }
        public string reason { get; set; }
        public int proto_ver { get; set; }
        public string proto_name { get; set; }
        public string ipaddress { get; set; }
        public long disconnected_at { get; set; }
        public string clientid { get; set; }
    }
}
