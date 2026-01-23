using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace IoTVideoService.Models
{
    public class In_HookNotFound
    {
        /// <summary>
        /// 服务器 id,通过配置文件设置
        /// </summary>
        public string mediaServerId { get; set; }
        /// <summary>
        /// 流应用名
        /// </summary>
        public string app { get; set; }
        /// <summary>
        /// TCP 链接唯一 ID
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 播放器 ip
        /// </summary>
        public string ip { get; set; }
        /// <summary>
        /// 播放 url 参数
        /// </summary>
        [JsonPropertyName("params")]
        public string @params { get; set; }
        /// <summary>
        /// 播放器端口号
        /// </summary>
        public int port { get; set; }
        /// <summary>
        /// 播放的协议，可能是 rtsp、rtmp
        /// </summary>
        public string schema { get; set; }
        /// <summary>
        /// 流 ID
        /// </summary>
        public string stream { get; set; }
        /// <summary>
        /// 流虚拟主机
        /// </summary>
        public string vhost { get; set; }
    }
}
