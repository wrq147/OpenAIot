using System;

namespace ChannelUtility.Message
{
    public class MediaUserVerifyMessage : BaseDeviceMessage
    {
        public MediaUserVerifyMessage()
        {
            MsgType = "MediaUser";
        }
        public string UserName { get; set; }
    }
}
