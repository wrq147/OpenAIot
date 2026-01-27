using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelUtility.Message
{
    public class MediaRecordStartMessage : BaseDeviceMessage
    {
        public MediaRecordStartMessage()
        {
            MsgType = "MediaRecS";
        }
        public byte Storage { get; set; }
        public string Date { get; set; }
        public string FileId { get; set; }
        public string StreamId { get; set; }
    }
}
