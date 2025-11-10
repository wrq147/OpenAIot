using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelUtility.Message
{
    public class StartReadAllMessage : BaseUpDeviceMessage
    {
        public List<string> props { get; set; }
        public StartReadAllMessage()
        {
            MsgType = "StartReadAll";
        }
    }
}
