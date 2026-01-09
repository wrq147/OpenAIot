using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GB28181Channel.GB28181.Enum
{
    public enum StreamState
    {
        Inviting,    // 已发送INVITE
        Connecting,  // 设备正在处理
        Streaming,   // 流已建立
        Stopped,     // 已停止
        Failed       // 失败
    }
}
