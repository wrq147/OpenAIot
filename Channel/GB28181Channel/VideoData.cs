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
        public List<BoxItem> BoxList { get; set; }
        public List<AIConfigData> Configs { get; set; }
        public bool NeedUp { get; set; }
    }
}
