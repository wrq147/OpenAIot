
namespace ChannelUtility.Message
{
    /// <summary>
    /// 生成临时设备与协议关联缓存
    /// </summary>
    public class TempProductMessage : BaseUpDeviceMessage
    {
        public TempProductMessage()
        {
            MsgType = "TempProduct";
        }
    }
}
