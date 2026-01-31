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
        public List<FileRecord> Records { get; set; }
    }
    public class FileRecord
    {
        public string StreamId { get; set; }
        public string Date { get; set; }
        public string FileName { get; set; }
    }
}
