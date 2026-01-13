using GB28181Channel.GB28181.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GB28181Channel.GB28181.DTO
{
    /// <summary>
    /// 通道信息DTO
    /// </summary>
    public class ChannelInfo
    {
        public int Index { get; set; }
        public string PushKey { get; set; }
        public string DeviceId { get; set; }
        public DateTime InviteTime { get; set; }
        public StreamState SessionStatus { get; set; }
        public string Ssrc { get; set; }
        public int RemoteRtpPort { get; set; }
        public string ChannelId { get; set; }
        public string ChannelName { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public string Status { get; set; } // ON/OFF
    }
}
