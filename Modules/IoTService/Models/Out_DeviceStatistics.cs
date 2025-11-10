using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTService.Models
{
    public class Out_DeviceStatistics
    {
        /// <summary>
        /// 总设备数
        /// </summary>
        public int TotalCount { get; set; }
        /// <summary>
        /// 在线数
        /// </summary>
        public int OnlineCount { get; set; }
        /// <summary>
        /// 离线数
        /// </summary>
        public int OfflineCount { get; set; }
        /// <summary>
        /// 未知数
        /// </summary>
        public int UnknowCount { get; set; }
        /// <summary>
        /// 待处理事件数
        /// </summary>
        public int EventCount { get; set; }
    }
}
