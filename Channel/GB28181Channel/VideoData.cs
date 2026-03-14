using ChannelUtility.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace GB28181Channel
{
    public class VideoData
    {
        private List<BoxItem> _boxList;

        // 公开属性：每次赋新值
        public List<BoxItem> BoxList
        {
            get => Volatile.Read(ref _boxList);
            set => Volatile.Write(ref _boxList, value);
        }
        public VideoCaptureItem Item { get; set; }
        public float MotionRatio { get; set; }
        public int CoolDownMs { get; set; }

        public List<AIConfigData> Configs { get; set; }
        public bool NeedUp { get; set; }
    }
}
