using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelUtility.Message
{
    public class MediaRecordStopMessageReply : BaseDeviceMessage
    {
        public MediaRecordStopMessageReply()
        {
            MsgType = "MediaRecEOk";
        }
        public bool IsSuccess { get; set; }
        public string Reason { get; set; }
    }
}
