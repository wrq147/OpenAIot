using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedirectMqtt
{
    public class RedirectOption
    {
        /// <summary>
        /// 重定义DtuId
        /// </summary>
        public string[] DtuIds { get; set; }
        /// <summary>
        /// 重定向源mqtt_server
        /// </summary>
        public string Source_Server { get; set; }
        public int Source_Port { get; set; }
        /// <summary>
        /// 重定向源用户名
        /// </summary>
        public string Source_UserName { get; set; }
        /// <summary>
        /// 重定向源密码
        /// </summary>
        public string Source_Password { get; set; }
        /// <summary>
        /// 重定向目标
        /// </summary>
        public string Dest_Server { get; set; }
        public int Dest_Port { get; set; }
        /// <summary>
        /// 重定向目标用户名
        /// </summary>
        public string Dest_UserName { get; set; }
        /// <summary>
        /// 重定向目标密码
        /// </summary>
        public string Dest_Password { get; set; }
    }
}
