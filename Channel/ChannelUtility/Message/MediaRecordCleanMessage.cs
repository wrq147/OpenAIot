using System;
namespace ChannelUtility.Message
{
    public class MediaRecordCleanMessage : BaseDeviceMessage
    {
        public MediaRecordCleanMessage()
        {
            MsgType = "MediaClean";
        }
        public byte Storage { get; set; }
        public string StreamId { get; set; }
        public string Date { get; set; }
        public string FileName { get; set; }
    }
}
