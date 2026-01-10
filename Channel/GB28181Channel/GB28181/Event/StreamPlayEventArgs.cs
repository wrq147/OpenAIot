using GB28181Channel.GB28181.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GB28181Channel.GB28181.Event
{
    /// <summary>
    /// 点播事件参数
    /// </summary>
    public class StreamPlayEventArgs : EventArgs
    {
        public PlaybackParams Params { get; set; }
        /// <summary>
        /// 是否成功点播
        /// </summary>
        public bool IsSuccess { get; set; }
        public string SessionId { get; set; }
        public string Message { get; set; }
    }
}
