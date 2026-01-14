using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelUtility.Message
{
    public class MediaPTZMessageReply : BaseDeviceMessage
    {
        public MediaPTZMessageReply()
        {
            MsgType = "MediaPTZOk";
        }
        public bool IsSuccess { get; set; }
        public string Reason { get; set; }
    }
}
