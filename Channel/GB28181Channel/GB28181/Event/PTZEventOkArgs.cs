using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GB28181Channel.GB28181.Event
{
    public class PTZEventOkArgs : EventArgs
    {
        public string DeviceId { get; set; }
        public string MessageId { get; set; }
        public bool IsSuccess { get; set; }
        public string Reason { get; set; }
    }
}
