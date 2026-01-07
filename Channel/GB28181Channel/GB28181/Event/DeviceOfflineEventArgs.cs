using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GB28181Channel.GB28181.Event
{
    /// <summary>
    /// 设备离线事件参数
    /// </summary>
    public class DeviceOfflineEventArgs : EventArgs
    {
        public string DeviceId { get; set; }
        public DateTime OfflineTime { get; set; }
        public DateTime LastHeartbeat { get; set; }
        public string Reason { get; set; } = "心跳超时";
    }
}
