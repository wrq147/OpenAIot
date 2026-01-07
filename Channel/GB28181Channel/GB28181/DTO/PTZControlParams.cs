using GB28181Channel.GB28181.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GB28181Channel.GB28181.DTO
{
    /// <summary>
    /// PTZ控制参数DTO
    /// </summary>
    public class PTZControlParams
    {
        public string DeviceId { get; set; }
        public string ChannelId { get; set; }
        public PTZCommandType CommandType { get; set; }
        public int Speed { get; set; } = 5; // 速度1-10
        public int PresetId { get; set; } = 0; // 预置位ID
    }
}
