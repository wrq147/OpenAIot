using System;

namespace ChannelUtility.Message
{
    public class MediaNotFoundMessage : BaseDeviceMessage
    {
        public MediaNotFoundMessage()
        {
            MsgType = "MediaNF";
        }
        public string StreamId { get; set; }
        public string NodeId { get; set; }
    }
}
