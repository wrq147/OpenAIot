using ChannelUtility.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GB28181Channel
{
    public class VideoData
    {
        public VideoCaptureItem Item { get; set; }
        public float MotionRatio { get; set; }
        public int CoolDownMs { get; set; }
        public List<AIDetectorTask> DetectList { get; set; }
    }
}
