using System;
namespace ChannelUtility.Message
{
    /// <summary>
    /// 设备上线消息
    /// </summary>
    public class DeviceOnlineMessage : BaseUpDeviceMessage
    {
        /// <summary>
        /// 客户端Ip地址
        /// </summary>
        public string IpAddress { get; set; }
        public string NodeGuid { get; set; }
        public DeviceOnlineMessage()
        {
            MsgType = "Online";
        }
    }
}
