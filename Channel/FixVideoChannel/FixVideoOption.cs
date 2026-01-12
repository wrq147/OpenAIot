using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixVideoChannel
{
    public class FixVideoOption
    {
        public string event_conn { get; set; }
        public string event_user { get; set; }
        public string event_pass { get; set; }
        public string redis_conn { get; set; }
        public string node_id { get; set; }
        /// <summary>
        /// ZLMediaKit的RTMP播放端口
        /// </summary>
        public int RTMPPort { get; set; } = 1935;
    }
}
