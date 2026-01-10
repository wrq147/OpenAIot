using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GB28181Channel.GB28181.Enum
{
    public enum StreamState
    {
        None,
        Inviting,    // 已发送INVITE
        Playing,   // 流已建立
        Stopped,     // 已停止
        Failed       // 失败
    }
}
