using System;
namespace ChannelUtility.Message
{
    /// <summary>
    /// 设备离线消息
    /// </summary>
    public class DeviceOfflineMessage : BaseUpDeviceMessage
    {
        public DeviceOfflineMessage()
        {
            MsgType = "Offline";
        }
    }
}
