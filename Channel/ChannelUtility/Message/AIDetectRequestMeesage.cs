using System;
using System.Collections.Generic;

namespace ChannelUtility.Message
{
    public class AIDetectRequestMeesage : BaseDeviceMessage
    {
        public AIDetectRequestMeesage()
        {
            MsgType = "AIDetectReq";
        }
        public string VideoKey { get; set; }
        public string NodeId { get; set; }
        /// <summary>
        /// 运动区块占比
        /// </summary>
        public float MRatio { get; set; }
        public string Frame { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public List<AIConfigData> Configs { get; set; }
    }

    public class AIConfigData
    {
        public string DetType { get; set; }
        /// <summary>
        /// 是否启用绘制
        /// </summary>
        public bool IsDraw { get; set; }
        public Dictionary<string, object> DetParams { get; set; }
    }
}
