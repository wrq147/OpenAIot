using System;

namespace ChannelUtility.Message
{
    public class MediaNotFoundMessage : BaseDeviceMessage
    {
        public MediaNotFoundMessage()
        {
            MsgType = "MediaNF";
        }
        public int VideoType { get; set; }
        public string NodeGuid { get; set; }
        public string StreamId { get; set; }
    }
}
