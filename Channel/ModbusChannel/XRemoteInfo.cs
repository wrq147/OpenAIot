using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModbusChannel
{
    /// <summary>
    /// 远程配置信息
    /// </summary>
    public class XRemoteInfo
    {
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
}
