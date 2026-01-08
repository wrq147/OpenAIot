using GB28181Channel.GB28181.Enum;
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
        public string DtuId { get; set; }
        public string PushKey { get; set; }
        public string DeviceId { get; set; }
        public string DeviceIp { get; set; }
        public int DevicePort { get; set; }
        public DateTime RegisterTime { get; set; }
        public DateTime LastHeartbeatTime { get; set; }
        public GB28181Version ProtocolVersion { get; set; }

        /// <summary>
        /// 创建当前 DeviceInfo 对象的深拷贝副本
        /// </summary>
        /// <returns>新的 DeviceInfo 实例，包含当前对象的所有属性值</returns>
        public DeviceInfo Clone()
        {
            return new DeviceInfo
            {
                DtuId = this.DtuId,
                PushKey = this.PushKey,
                DeviceId = this.DeviceId,
                DeviceIp = this.DeviceIp,
                DevicePort = this.DevicePort,
                RegisterTime = this.RegisterTime,
                LastHeartbeatTime = this.LastHeartbeatTime,
                ProtocolVersion = this.ProtocolVersion
            };
        }

    }
}
