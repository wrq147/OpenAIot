using System;

namespace ChannelUtility.Message
{
    /// <summary>
    /// 设备绑定协议消息
    /// </summary>
    public class DeviceBindMessage : BaseDeviceMessage
    {
        public DeviceBindMessage()
        {
            MsgType = "Bind";
        }
    }
}
