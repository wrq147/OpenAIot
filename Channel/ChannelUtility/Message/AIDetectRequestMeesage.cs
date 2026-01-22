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
        public string VideoKey { get; set; }
        public string NodeGuid { get; set; }
        public string DetType { get; set; }
        /// <summary>
        /// 运动区块占比
        /// </summary>
        public float MRatio { get; set; }
        /// <summary>
        /// 是否启用绘制
        /// </summary>
        public bool IsDraw { get; set; }
        public Dictionary<string, object> DetParams { get; set; }
        public byte[] Frame { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
