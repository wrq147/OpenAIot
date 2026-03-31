using ChannelUtility.Message;
using GB28181Channel.GB28181.Enum;
using SIPSorcery.SIP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GB28181Channel.GB28181.DTO
{
    /// <summary>
    /// 设备信息DTO
    /// </summary>
    public class DeviceInfo
    {
        public VideoData VideoData { get; set; }
        public List<PresetInfo> PresetList { get; set; }
        public string DeviceId { get; set; }
        public string DeviceIp { get; set; }
        public int DevicePort { get; set; }
        public DateTime RegisterTime { get; set; }
        public DateTime LastHeartbeatTime { get; set; }
        public GB28181Version ProtocolVersion { get; set; }
        public SIPProtocolsEnum TransportProtocol { get; set; }
    }
}
