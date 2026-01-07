using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GB28181Channel.GB28181.Enum
{
    /// <summary>
    /// 录像类型
    /// </summary>
    public enum RecordType
    {
        Timing,    // 定时录像
        Manual,    // 手动录像
        Alarm,     // 报警录像
        Motion     // 移动侦测录像
    }
}
