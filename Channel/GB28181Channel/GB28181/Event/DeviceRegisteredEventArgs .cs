using GB28181Channel.GB28181.DTO;
using SIPSorcery.SIP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GB28181Channel.GB28181.Event
{
    /// <summary>
    /// 设备注册事件参数
    /// </summary>
    public class DeviceRegisteredEventArgs : EventArgs
    {
        public DeviceInfo Device { get; set; }
        public SIPRequest OriginalRequest { get; set; }
        public IPEndPoint RemoteEndPoint { get; set; }
    }
}
