using System;

namespace ChannelUtility.Message
{
    /// <summary>
    /// 设备绑定产品消息
    /// </summary>
    public class DeviceBindMessage : RequestMessage
    {
        public DeviceBindMessage()
        {
            MsgType = "Bind";
        }
    }
}
