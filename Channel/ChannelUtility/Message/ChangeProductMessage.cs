using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelUtility.Message
{
    /// <summary>
    /// 更改设备的所属协议
    /// </summary>
    public class ChangeProductMessage : BaseUpDeviceMessage
    {
        /// <summary>
        /// 变更后的协议Id
        /// </summary>
        public string TargetProductId { get; set; }
        public ChangeProductMessage()
        {
            MsgType = "ChangeProduct";
        }
    }
}
