using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ChannelUtility.Message
{
    public class AIDetectResponseMessage : BaseDeviceMessage
    {
        public AIDetectResponseMessage()
        {
            MsgType = "AIDetectResp";
        }
        public List<BoxItem> BoxList { get; set; }
        public bool NeedConf { get; set; }
    }
    /// <summary>
    /// 检测框
    /// </summary>
    public class BoxItem
    {
        public float x1 { get; set; }
        public float y1 { get; set; }
        public float x2 { get; set; }
        public float y2 { get; set; }
        public float score { get; set; }
        public string label { get; set; }
        public string color { get; set; }
        /// <summary>
        /// 关键点
        /// </summary>
        public List<KeyPoint> points { get; set; }
        /// <summary>
        /// 特征ID
        /// </summary>
        [JsonIgnore]
        public float[] ReID { get; set; }
    }
    public class KeyPoint
    {
        public float x { get; set; }
        public float y { get; set; }
        public float score { get; set; }
    }
}
