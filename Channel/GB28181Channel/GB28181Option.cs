using System;
namespace GB28181Channel
{
    public class GB28181Option
    {
        public string event_conn { get; set; }
        public string redis_conn { get; set; }
        public bool ipv6_enable { get; set; }
    }
}
