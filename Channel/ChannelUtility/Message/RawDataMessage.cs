using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelUtility.Message
{
    /// <summary>
    /// 原数据下发
    /// </summary>
    public class RawDataMessage : BaseDeviceMessage
    {
        public byte[] Data { get; set; }
        /// <summary>
        /// 边缘设备的下属设备
        /// </summary>
        public string prefix { get; set; }
        public RawDataMessage()
        {
            MsgType = "RawData";
        }
    }
}
