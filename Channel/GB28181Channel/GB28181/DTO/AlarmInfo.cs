using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GB28181Channel.GB28181.DTO
{
    /// <summary>
    /// 报警信息DTO
    /// </summary>
    public class AlarmInfo
    {
        public string AlarmId { get; set; }
        public string DeviceId { get; set; }
        public string ChannelId { get; set; }
        public string AlarmType { get; set; } // 移动侦测、越界、入侵等
        public DateTime AlarmTime { get; set; }
        public string AlarmDescription { get; set; }
        public bool IsHandled { get; set; }
    }
}
