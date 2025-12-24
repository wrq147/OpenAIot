using FFmpeg.AutoGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixVideoChannel
{
    public class FixVideoOption
    {
        public string event_conn { get; set; }
        public string redis_conn { get; set; }
        public string ffmpeg_path { get; set; }
        public string zlmedia_path { get; set; }
        public ZLMediaKitItem zlmedia_server { get; set; }
    }
    public class ZLMediaKitItem
    {
        /// <summary>
        /// ZLMediaKit接口密钥
        /// </summary>
        public string Secret { get; set; }
        /// <summary>
        /// ZLMediaKit的RTSP端口号
        /// </summary>
        public int RTSPPort { get; set; } = 554;
        /// <summary>
        /// ZLMediaKit的RTMP端口号
        /// </summary>
        public int RTMPPort { get; set; } = 1935;
        /// <summary>
        /// ZLMediaKit的应用名
        /// </summary>
        public string App { get; set; } = "live";
    }
}
