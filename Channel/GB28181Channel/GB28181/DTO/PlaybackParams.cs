using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GB28181Channel.GB28181.DTO
{
    /// <summary>
    /// 点播参数DTO
    /// </summary>
    public class PlaybackParams
    {
        public string DeviceId { get; set; }
        public string ChannelId { get; set; }
        public string Ssrc { get; set; }
        public int LocalRtpPort { get; set; }
        public string RemoteIp { get; set; }
        public int RemoteRtpPort { get; set; }
        public bool IsLive { get; set; } // true:实时直播 false:录像回放
        public DateTime? StartTime { get; set; } // 回放开始时间
        public DateTime? EndTime { get; set; } // 回放结束时间
    }
}
