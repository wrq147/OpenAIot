using System;
namespace ChannelUtility.Message
{
    public class BaseDeviceMessage
    {
        /// <summary>
        /// 消息类型
        /// </summary>
        public string MsgType { get; set; }
        /// <summary>
        /// 产品Id
        /// </summary>
        public string ProductId { get; set; }
        /// <summary>
        /// 设备dtuId
        /// </summary>
        public string DeviceId { get; set; }
    }
}
