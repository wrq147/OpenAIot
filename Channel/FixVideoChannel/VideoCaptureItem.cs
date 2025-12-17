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
        /// <summary>
        /// 推流地址
        /// </summary>
        public string PushAddr { get; set; }
        /// <summary>
        /// AI检测帧间隔,默认25帧
        /// </summary>
        public int FrameInterval { get; set; }
    }
}
