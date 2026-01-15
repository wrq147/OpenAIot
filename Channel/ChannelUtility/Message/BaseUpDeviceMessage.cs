using System;
using System.Collections.Generic;
namespace ChannelUtility.Message
{
    public class BaseUpDeviceMessage : BaseDeviceMessage
    {
        /// <summary>
        /// 来源通道
        /// </summary>
        public string NodeGuid { get; set; }
        /// <summary>
        /// 设备向事件总线发送消息的时间戳
        /// </summary>
        public long Timestamp { get; set; }
        /// <summary>
        /// 如果是转发的消息将转入源ProductId
        /// </summary>
        public string RedirectFromProductId { get; set; }
        /// <summary>
        /// 如果是转发的消息将转入源DtuId
        /// </summary>
        public string RedirecDtuId { get; set; }
        /// <summary>
        /// 触发的规则Id链
        /// </summary>
        public HashSet<long> RuleIds { get; set; }
    }
}
