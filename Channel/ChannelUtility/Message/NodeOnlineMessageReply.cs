using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelUtility.Message
{
    public class NodeOnlineMessageReply : BaseDeviceMessage
    {
        public NodeOnlineMessageReply()
        {
            MsgType = "NodeOnReply";
        }
    }
}
