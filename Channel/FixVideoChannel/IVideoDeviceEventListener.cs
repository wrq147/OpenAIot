using ChannelUtility;
using ChannelUtility.Message;

namespace FixVideoChannel
{
    public interface IVideoDeviceEventListener
    {
        Task OnEventOffline(VideoCaptureItem item);
        Task OnEventOnline(VideoCaptureItem item);
        Task OnSendAIDetectRequest(string videoId, string videoKey, float motionRatio, byte[] pressData, int width, int height, List<AIConfigData> confs);
        Task OnDeviceDownMessage(BaseDeviceMessage msg);
    }
}
