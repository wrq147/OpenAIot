using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelUtility.Message
{
    /// <summary>
    /// 空消息回复，系统总线不处理的消息
    /// </summary>
    public class EmptyMessageReply : BaseUpDeviceMessage
    {
        public EmptyMessageReply()
        {
            MsgType = "Empty";
        }
    }
}
