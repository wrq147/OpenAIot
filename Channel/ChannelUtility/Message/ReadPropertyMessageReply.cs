using System;
using System.Collections.Generic;

namespace ChannelUtility.Message
{
    /// <summary>
    /// 设备属性上报消息
    /// </summary>
    public class ReadPropertyMessageReply : BaseUpDeviceMessage
    {
        /// <summary>
        /// 属性键值对
        /// </summary>
        public IDictionary<string, object> Properties { get; set; }
        /// <summary>
        /// 是否为标签同步到属性的消息
        /// </summary>
        public bool IsTagSync { get; set; }
        public ReadPropertyMessageReply()
        {
            MsgType = "ReadPropertyReply";
        }
    }
}
