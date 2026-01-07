using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GB28181Channel.GB28181.Enum
{
    /// <summary>
    /// SIP传输协议枚举
    /// </summary>
    public enum SIPTransportProtocol
    {
        /// <summary>仅使用UDP协议</summary>
        UdpOnly,
        /// <summary>仅使用TCP协议</summary>
        TcpOnly,
        /// <summary>同时使用UDP和TCP协议（默认）</summary>
        Both
    }
}
