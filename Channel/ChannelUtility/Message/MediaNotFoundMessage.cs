using System;

namespace ChannelUtility.Message
{
    public class MediaNotFoundMessage : BaseDeviceMessage
    {
        public MediaNotFoundMessage()
        {
            MsgType = "MediaNF";
        }
        /// <summary>
        /// 0为固定视频，1为gb28181,3为Onvif设备
        /// </summary>
        public int VideoType { get; set; }
        public string StreamId { get; set; }
    }
}
