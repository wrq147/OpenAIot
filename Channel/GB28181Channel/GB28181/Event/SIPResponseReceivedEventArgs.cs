using GB28181Channel.GB28181.DTO;
using SIPSorcery.SIP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GB28181Channel.GB28181.Event
{
    public class SIPResponseReceivedEventArgs : EventArgs
    {
        public SIPResponse Response { get; set; }
        public SIPEndPoint RemoteEndPoint { get; set; }
        public string TransportProtocol { get; set; }
        public RequestContext RequestContext { get; set; }
    }
}
