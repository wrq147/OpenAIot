using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTVideoService
{
    public class VideoOption
    {
        /// <summary>
        /// ZLMediaKit接口密钥
        /// </summary>
        public string ZLMediaKitSecret { get; set; }
        /// <summary>
        /// ZLMediaKit的Ip地址
        /// </summary>
        public string ZLMediaKitIp { get; set; }
        /// <summary>
        /// ZLMediaKit的RTSP端口号
        /// </summary>
        public int ZLMediaKitRTSPPort { get; set; } = 8554;
        /// <summary>
        /// ZLMediaKit的RTMP端口号
        /// </summary>
        public int ZLMediaKitRTMPPort { get; set; } = 1935;
        /// <summary>
        /// ZLMediaKit的应用名
        /// </summary>
        public string ZLMediaKitApp { get; set; } = "live";
    }
}
