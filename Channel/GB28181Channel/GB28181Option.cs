using System;
namespace GB28181Channel
{
    public class GB28181Option
    {
        public string event_conn { get; set; }
        public string event_user { get; set; }
        public string event_pass { get; set; }
        public string redis_conn { get; set; }
        public string sip_service_id { get; set; }
        public string sip_ip { get; set; }
        public int sip_port { get; set; }
        /// <summary>
        /// 消息协议：udp、tcp、both
        /// </summary>
        public string sip_protocol { get; set; }
        public int rtp_port { get; set; }
        /// <summary>
        /// ZLMediaKit的RTMP播放端口
        /// </summary>
        public int rtmp_port { get; set; }
        public int http_port { get; set; }
        public string minio_server { get; set; }
        public string minio_access { get; set; }
        public string minio_secret { get; set; }
        public string minio_bucket { get; set; }
        public string minio_url { get; set; }
    }
}
