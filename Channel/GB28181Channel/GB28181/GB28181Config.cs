using GB28181Channel.GB28181.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GB28181Channel.GB28181
{
    /// <summary>
    /// GB28181配置类
    /// </summary>
    public class GB28181Config
    {
        public GB28181Version Version { get; set; } = GB28181Version.V2016;
        public string ServerId { get; set; } = "34020000002000000001";
        public int SipPort { get; set; } = 13881;
        public string ServerIp { get; set; } = "0.0.0.0";
        public int HeartbeatTimeoutSeconds { get; set; } = 300; // 心跳超时时间(5分钟)
        public int DefaultRtpPort { get; set; } = 58200; // 默认RTP接收端口
    }
}
