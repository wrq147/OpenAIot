using System;
namespace MqttChannel
{
    public class MqttOption
    {
        public string event_conn { get; set; }
        public string event_user { get; set; }
        public string event_pass { get; set; }
        public string redis_conn { get; set; }
        /// <summary>
        /// 可以指定emqx服务器或传空使用本地mqtt服务
        /// </summary>
        public string mqtt_server { get; set; }
        public int mqtt_port { get; set; }
        public string mqtt_username { get; set; }
        public string mqtt_password { get; set; }
        /// <summary>
        /// 下发定时延时线程数：为0则不延时
        /// </summary>
        public int run_count { get; set; }
        /// <summary>
        /// 轮询间隔
        /// </summary>
        public int send_interval { get; set; } = 400;
    }
}
