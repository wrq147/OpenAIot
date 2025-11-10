using System;

namespace MqttService
{
    public class MqttOption
    {
        /// <summary>
        /// 是否启用emqx：否时则使用内置mqtt
        /// </summary>
        public bool enable_emqx { get; set; }
        /// <summary>
        /// 节点名称
        /// </summary>
        public string node_name { get; set; }
        /// <summary>
        /// 服务端Tcp连接服务器地址
        /// </summary>
        public string mqtt_tcp_server { get; set; }
        /// <summary>
        /// 服务端Tcp连接端口
        /// </summary>
        public int mqtt_tcp_port { get; set; }
        /// <summary>
        /// 服务端Websocket连接端口
        /// </summary>
        public int mqtt_web_port { get; set; }
        /// <summary>
        /// 服务端SSL的Websocket连接端口
        /// </summary>
        public int mqtt_web_ssl_port { get; set; }
        /// <summary>
        /// 系统账号
        /// </summary>
        public string sys_username { get; set; }
        /// <summary>
        /// 系统密码
        /// </summary>
        public string sys_password { get; set; }
    }
}
