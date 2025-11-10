using System;

namespace ChannelUtility.Message
{
    /// <summary>
    /// 设备绑定产品消息回复
    /// </summary>
    public class DeviceBindMessageReply : BaseUpDeviceMessage
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess { get; set; }
        /// <summary>
        /// 失败原因
        /// </summary>
        public string Reason { get; set; }
        public DeviceBindMessageReply()
        {
            MsgType = "BindReply";
        }
    }
}
