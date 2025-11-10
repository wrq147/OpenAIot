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
        /// 是否被处理
        /// </summary>
        public bool IsDone { get; set; }
    }
}
