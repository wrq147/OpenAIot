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
        public string DeviceId { get; set; }
        public string Password { get; set; }
        public string DeviceIp { get; set; }
        public int DevicePort { get; set; }
        public DateTime RegisterTime { get; set; }
        public DateTime LastHeartbeatTime { get; set; }
        public GB28181Version ProtocolVersion { get; set; }
        public SIPProtocolsEnum TransportProtocol { get; set; }
        /// <summary>
        /// 创建当前 DeviceInfo 对象的深拷贝副本
        /// </summary>
        /// <returns>新的 DeviceInfo 实例，包含当前对象的所有属性值</returns>
        public DeviceInfo Clone()
        {
            return new DeviceInfo
            {
                VideoData = this.VideoData,
                DeviceId = this.DeviceId,
                Password = this.Password,
                DeviceIp = this.DeviceIp,
                DevicePort = this.DevicePort,
                RegisterTime = this.RegisterTime,
                LastHeartbeatTime = this.LastHeartbeatTime,
                ProtocolVersion = this.ProtocolVersion,
                TransportProtocol = this.TransportProtocol
            };
        }

    }
}
