using System;
using System.Collections.Generic;

namespace ChannelUtility.Message
{
    /// <summary>
    /// 执行规则引擎的消息
    /// </summary>
    public class ExecuteRuleMessage : BaseDeviceMessage
    {
        /// <summary>
        /// 其它自定义参数
        /// </summary>
        public IDictionary<string, object> Inputs { get; set; }
        /// <summary>
        /// 触发方式
        /// </summary>
        public byte TriggerWay { get; set; }
        public ExecuteRuleMessage()
        {
            MsgType = "Execute";
        }
    }
}
