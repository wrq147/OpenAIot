using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelUtility.Message
{
    public class AIDetectResponseMessage: RequestMessage
    {
        public AIDetectResponseMessage()
        {
            MsgType = "AIDetectResp";
        }
    }
}
