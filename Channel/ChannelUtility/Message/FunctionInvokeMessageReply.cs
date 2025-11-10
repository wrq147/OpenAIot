using System;
using System.Collections.Generic;

namespace ChannelUtility.Message
{
    /// <summary>
    /// 调用设备功能的消息
    /// </summary>
    public class FunctionInvokeMessageReply : BaseUpDeviceMessage
    {
        public string MessageId { get; set; }
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess { get; set; }
        /// <summary>
        /// 错误原因
        /// </summary>
        public string Error { get; set; }

        public IDictionary<string,object> Outputs { get; set; }

        public FunctionInvokeMessageReply()
        {
            MsgType = "FunctionReply";
        }
    }
}
