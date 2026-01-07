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
        public string DeviceId { get; set; }
        public string DeviceName { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public string FirmwareVersion { get; set; }
        public string DeviceIp { get; set; }
        public int DevicePort { get; set; }
        public DeviceStatus Status { get; set; }
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
                DeviceId = this.DeviceId,
                DeviceName = this.DeviceName,
                Manufacturer = this.Manufacturer,
                Model = this.Model,
                FirmwareVersion = this.FirmwareVersion,
                DeviceIp = this.DeviceIp,
                DevicePort = this.DevicePort,
                Status = this.Status,
                RegisterTime = this.RegisterTime,
                LastHeartbeatTime = this.LastHeartbeatTime,
                ProtocolVersion = this.ProtocolVersion
            };
        }

    }
}
