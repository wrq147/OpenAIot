using Microsoft.VisualBasic;
using System;

namespace TemplateAction.Core
{
    /// <summary>
    /// 等待返回的事件
    /// </summary>
    public class ResponseEvent
    {
        public string MessageId { get; set; }
        public ResponseEvent()
        {
            this.MessageId = Guid.NewGuid().ToString("N");
        }
    }
    /// <summary>
    /// 事件返回结果
    /// </summary>
    public class EvtResponse
    {
        /// <summary>
        /// 是否处理
        /// </summary>
        public bool IsDone { get; set; }
        /// <summary>
        /// 错误代码：0为成功，-1为超时，其它为失败
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string Message { get; set; }
    }
}
