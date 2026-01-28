using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelUtility.Message
{
    public class NodeOnlineMessage : BaseDeviceMessage
    {
        public NodeOnlineMessage()
        {
            MsgType = "NodeOn";
        }
        /// <summary>
        /// 通道代码
        /// </summary>
        public string ChannelCode { get; set; }
    }
}
