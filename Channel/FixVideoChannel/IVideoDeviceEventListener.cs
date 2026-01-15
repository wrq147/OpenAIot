using ChannelUtility;
using ChannelUtility.Message;

namespace FixVideoChannel
{
    public interface IVideoDeviceEventListener
    {
        Task OnEventOffline(VideoCaptureItem item);
        Task OnEventOnline(VideoCaptureItem item);
        Task OnSendAIDetectRequest(string videoId, AIDetectItem item, float motionRatio, byte[] pressData, int width, int height);
        Task OnDeviceDownMessage(BaseDeviceMessage msg);
    }
}
