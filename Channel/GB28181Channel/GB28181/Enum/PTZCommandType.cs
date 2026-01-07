using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GB28181Channel.GB28181.Enum
{
    /// <summary>
    /// PTZ控制命令类型
    /// </summary>
    public enum PTZCommandType
    {
        Up, Down, Left, Right, ZoomIn, ZoomOut, Stop,
        PresetSet, PresetGoto, PresetClear
    }
}
