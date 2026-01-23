using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelUtility.Message
{
    public class AIDetectResponseMessage: BaseDeviceMessage
    {
        public AIDetectResponseMessage()
        {
            MsgType = "AIDetectResp";
        }
        public List<BoxItem> BoxList { get; set; }
        public bool NeedConf { get; set; }
    }
    public class BoxItem
    {
        public float x1 { get; set; }
        public float y1 { get; set; }
        public float x2 { get; set; }
        public float y2 { get; set; }
        public float score { get; set; }
        public string label { get; set; }
        public string color { get; set; }
    }
}
