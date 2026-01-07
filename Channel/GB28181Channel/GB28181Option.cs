using System;
namespace GB28181Channel
{
    public class GB28181Option
    {
        public string event_conn { get; set; }
        public string redis_conn { get; set; }
        public string sip_service_id { get; set; }
        public string sip_ip { get; set; }
        public int sip_port { get; set; }
        /// <summary>
        /// 消息协议：udp、tcp、both
        /// </summary>
        public string sip_protocol { get; set; }
    }
}
