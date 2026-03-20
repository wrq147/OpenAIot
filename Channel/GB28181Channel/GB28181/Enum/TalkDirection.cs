using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GB28181Channel.GB28181.Enum
{
    public enum TalkDirection
    {
        // 服务器主动发起对讲（服务器→设备）
        ServerToDevice,
        // 设备主动发起对讲（设备→服务器）
        DeviceToServer
    }
}
