using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTVideoService.Models
{
    public class In_HookNoneReader
    {
        /// <summary>
        /// 流应用名
        /// </summary>
        public string app { get; set; }
        /// <summary>
        /// rtsp 或 rtmp
        /// </summary>
        public string schema { get; set; }
        /// <summary>
        /// 流ID
        /// </summary>
        public string stream { get; set; }
        /// <summary>
        /// 流虚拟主机
        /// </summary>
        public string vhost { get; set; }
        /// <summary>
        /// 服务器 id,通过配置文件设置
        /// </summary>
        public string mediaServerId { get; set; }
    }
}
