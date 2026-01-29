using ChannelUtility;
using ChannelUtility.Message;

namespace FixVideoChannel
{
    public interface IVideoDeviceEventListener
    {
        Task OnSendRecordFile(string videoId, string videoKey, string fileName, ulong fileSize, ulong startTime, float timeLen, byte storage);
        Task OnEventOffline(VideoCaptureItem item);
        Task OnEventOnline(VideoCaptureItem item);
        Task OnSendAIDetectRequest(string videoId, string videoKey, float motionRatio, byte[] pressData, int width, int height, List<AIConfigData> confs);
        Task OnDeviceDownMessage(BaseDeviceMessage msg);
    }
}
