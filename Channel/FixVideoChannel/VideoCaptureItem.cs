using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixVideoChannel
{
    public class VideoCaptureItem
    {
        /// <summary>
        /// 视频Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 拉流地址
        /// </summary>
        public string PullAddr { get; set; }
    }
}
