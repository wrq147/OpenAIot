using System;

namespace ChannelUtility.Message
{
    public class MediaNotFoundMessage : BaseDeviceMessage
    {
        public MediaNotFoundMessage()
        {
            MsgType = "MediaNF";
        }
        public string NodeGuid { get; set; }
        public string NodeId { get; set; }
    }
}
