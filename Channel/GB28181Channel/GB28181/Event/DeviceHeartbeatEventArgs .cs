using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GB28181Channel.GB28181.Event
{
    /// <summary>
    /// 设备心跳事件参数
    /// </summary>
    public class DeviceHeartbeatEventArgs : EventArgs
    {
        public string DeviceId { get; set; }
        public DateTime HeartbeatTime { get; set; }
        public IPEndPoint RemoteEndPoint { get; set; }
    }
}
