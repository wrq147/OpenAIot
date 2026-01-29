using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelUtility.Message
{
    public class MediaRecordFileMessage : BaseDeviceMessage
    {
        public MediaRecordFileMessage()
        {
            MsgType = "MediaFile";
        }
        public string NodeId { get; set; }
        public string StreamId { get; set; }
        public string FileName { get; set; }
        public ulong FileSize { get; set; }
        public ulong StartTime { get; set; }
        public float TimeLen { get; set; }
        public byte Storage { get; set; }
        /// <summary>
        /// 0为mp4、1为hls
        /// </summary>
        public byte SaveType { get; set; }
    }
}
