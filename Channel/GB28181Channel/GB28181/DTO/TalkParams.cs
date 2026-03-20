using GB28181Channel.GB28181.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GB28181Channel.GB28181.DTO
{
    // 对讲控制参数
    public class TalkParams
    {
        /// <summary>
        /// 设备ID
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        /// 通道ID
        /// </summary>
        public string ChannelId { get; set; }

        /// <summary>
        /// 本地RTP端口（接收设备音频）
        /// </summary>
        public int LocalRtpPort { get; set; }

        /// <summary>
        /// 对讲方向
        /// </summary>
        public TalkDirection Direction { get; set; }

        /// <summary>
        /// 音频编码（默认G.711A）
        /// </summary>
        public string AudioCodec { get; set; } = "PCMA/8000";

        /// <summary>
        /// 会话ID
        /// </summary>
        public string SessionId { get; set; } = Guid.NewGuid().ToString("N");
    }
}
