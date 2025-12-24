using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelUtility.Message
{
    public class UpVideoItemMessage : BaseDeviceMessage
    {
        public UpVideoItemMessage()
        {
            MsgType = "UpVItem";
        }
        public VideoCaptureItem Item { get; set; }
        public List<AIDetectItem> DetectList { get; set; }
    }
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
        /// 推流的流Id
        /// </summary>
        public string PushKey { get; set; }
        /// <summary>
        /// AI检测帧间隔,默认25帧
        /// </summary>
        public int FrameInterval { get; set; }
    }
    public class AIDetectItem
    {
        /// <summary>
        /// 检测类型
        /// </summary>
        public string DetectType { get; set; }
        /// <summary>
        /// 是否启用绘制
        /// </summary>
        public bool EnableDraw { get; set; }
        /// <summary>
        /// 检测参数
        /// </summary>
        public Dictionary<string,string> DetectParams { get; set; }
    }
}
