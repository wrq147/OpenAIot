using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MqttChannel
{
    public class MqttOption
    {
        public string event_conn { get; set; }
        public string redis_conn { get; set; }
        public string mqtt_server { get; set; }
        public int mqtt_port { get; set; }
        public string mqtt_username { get; set; }
        public string mqtt_password { get; set; }
        /// <summary>
        /// 下发定时延时线程数：为0则不延时
        /// </summary>
        public int run_count { get; set; }
    }
}
