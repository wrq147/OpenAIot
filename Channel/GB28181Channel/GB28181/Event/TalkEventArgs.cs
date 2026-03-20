using GB28181Channel.GB28181.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GB28181Channel.GB28181.Event
{
    public class TalkEventArgs : EventArgs
    {
        /// <summary>
        /// 对讲参数
        /// </summary>
        public TalkParams Params { get; set; }

        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 会话ID
        /// </summary>
        public string SessionId { get; set; }

        /// <summary>
        /// 消息描述
        /// </summary>
        public string Message { get; set; }
    }
}
