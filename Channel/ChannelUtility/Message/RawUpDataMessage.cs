using EasyNetQ;
using System;

namespace ChannelUtility.Message
{
    [QueueAttribute("raw_up_queue", ExchangeName = "raw_up_exchange")]
    public class RawUpDataMessage : BaseDeviceMessage
    {
        public byte[] Data { get; set; }
        /// <summary>
        /// 边缘设备的下属设备
        /// </summary>
        public string prefix { get; set; }
        public string NodeId { get; set; }
        public RawUpDataMessage()
        {
            MsgType = "RawUpData";
        }
    }
}
