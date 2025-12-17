using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelUtility.Message
{
    public class AIDetectRequestMeesage : BaseDeviceMessage
    {
        public AIDetectRequestMeesage()
        {
            MsgType = "AIDetectReq";
        }
        public string DetType { get; set; }
        public Dictionary<string, string> DetParams { get; set; }
        public byte[] RgbFrame { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
