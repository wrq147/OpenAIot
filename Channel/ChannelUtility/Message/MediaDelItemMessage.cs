using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelUtility.Message
{
    public class MediaDelItemMessage : BaseDeviceMessage
    {
        public MediaDelItemMessage()
        {
            MsgType = "DelVItem";
        }
    }
}
