using System;
using System.Collections.Generic;

namespace ChannelUtility.Message
{
    /// <summary>
    /// 设备事件消息
    /// </summary>
    public class DeviceEventMessage : BaseUpDeviceMessage
    {
        public DeviceEventMessage()
        {
            MsgType = "Event";
        }
        /// <summary>
        /// 事件标识,在元数据中定义
        /// </summary>
        public string EventId { get; set; }
        /// <summary>
        /// 事件数据
        /// </summary>
        public IDictionary<string, object> Outputs { get; set; }

    }
}
