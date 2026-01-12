using System;
namespace ModbusChannel
{
    public class ModbusOption
    {
        public string event_conn { get; set; }
        public string event_user { get; set; }
        public string event_pass { get; set; }
        public string redis_conn { get; set; }
        /// <summary>
        /// 网络连接方式：0为自动选择、1为串口通信、2为tcp客户端通信、3为tcp服务端通信
        /// </summary>
        public int net_way { get; set; }
        /// <summary>
        /// 串口配置
        /// </summary>
        public SerialItem[] serial_names { get; set; }
        /// <summary>
        /// gpio的dtuid
        /// </summary>
        public string gpio_dtuid { get; set; }
        /// <summary>
        /// 报文间隔
        /// </summary>
        public int down_interval { get; set; }
        /// <summary>
        /// 轮询间隔
        /// </summary>
        public int send_interval { get; set; }
        /// <summary>
        /// tcp的dtuid
        /// </summary>
        public string tcp_dtuid { get; set; }
        /// <summary>
        /// tcp通讯的ip
        /// </summary>
        public string tcp_ip { get; set; }
        /// <summary>
        /// tcp通讯的端口号
        /// </summary>
        public int tcp_port { get; set; }
        /// <summary>
        /// 服务器地址
        /// </summary>
        public string server_ip { get; set; }
        /// <summary>
        /// 服务端远控端口
        /// </summary>
        public int server_port { get; set; }
        /// <summary>
        /// 服务端令牌
        /// </summary>
        public string server_token { get; set; }
        /// <summary>
        /// MD5加密的开发者密钥
        /// </summary>
        public string server_devkey { get; set; }
        /// <summary>
        /// mqtt端口号
        /// </summary>
        public int mqtt_port { get; set; }
        /// <summary>
        /// mqtt账号
        /// </summary>
        public string mqtt_username { get; set; }
        /// <summary>
        /// mqtt密码
        /// </summary>
        public string mqtt_password { get; set; }

    }
    public class SerialItem
    {
        /// <summary>
        /// 串口名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 设备通讯ID
        /// </summary>
        public string dtuid { get; set; }
    }
}
