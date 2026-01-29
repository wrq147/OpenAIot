using System;
using System.Collections.Generic;
using System.IO;
namespace ChannelUtility.Message
{
    public class MediaRecordCleanMessage : BaseDeviceMessage
    {
        public MediaRecordCleanMessage()
        {
            MsgType = "MediaClean";
        }
        public List<string> StreamIds { get; set; }
        public List<string> Dates { get; set; }
    }
}
