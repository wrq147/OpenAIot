using System;
using System.Collections.Generic;

namespace ChannelUtility.Message
{
    /// <summary>
    /// 调用设备功能的消息
    /// </summary>
    public class FunctionInvokeMessage : RequestMessage
    {
        /// <summary>
        /// 通过其它功能执行时，会传入源功能
        /// </summary>
        public string SourceFunction { get; set; }
        public string FunctionId { get; set; }//功能标识
        public IDictionary<string, object> Inputs { get; set; }
        public FunctionInvokeMessage()
        {
            MsgType = "Function";
        }
    }

}
