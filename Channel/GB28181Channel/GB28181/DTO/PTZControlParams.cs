using ChannelUtility.Message;
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
        public string MessageId { get; set; }
        public PTZCommandType CommandType { get; set; }
        /// <summary>
        /// 速度默认16（16进制10）
        /// </summary>
        public byte Speed { get; set; } = 16;
        /// <summary>
        /// 预置位ID(仅预置位指令有效)
        /// </summary>
        public byte? PresetId { get; set; }
    }
}
