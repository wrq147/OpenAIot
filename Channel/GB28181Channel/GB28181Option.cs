using System;
namespace GB28181Channel
{
    public class GB28181Option
    {
        public string event_conn { get; set; }
        public string redis_conn { get; set; }
        public bool ipv6_enable { get; set; }
        public string sip_service_id { get; set; }
        public ushort sip_listen_port { get; set; }
    }
}
