using System;

namespace ChannelUtility.Message
{
    public class RawUpDataMessage : BaseDeviceMessage
    {
        public byte[] Data { get; set; }
        /// <summary>
        /// 边缘设备的下属设备
        /// </summary>
        public string prefix { get; set; }
        public string NodeGuid { get; set; }
        public bool IsReturn { get; set; }
        public RawUpDataMessage()
        {
            MsgType = "RawUpData";
        }
    }
}
