using ChannelUtility;
using ChannelUtility.Message;
using Microsoft.Extensions.DependencyInjection;

namespace FixVideoChannel
{
    public class FixVideoDeviceEventListener : IVideoDeviceEventListener
    {
        private IServiceProvider _serviceProvider;
        public FixVideoDeviceEventListener(IServiceProvider provider)
        {
            _serviceProvider = provider;
        }


        public async Task OnEventOffline(VideoCaptureItem item)
        {
            var eventBus = _serviceProvider.GetService<ClientBusProxy>();
            await eventBus.Disconnect(item.Id);
            ZLMediaKitServer.Instance.RemovePullProxy(item.Id);
        }

        public async Task OnEventOnline(VideoCaptureItem item)
        {
            var eventBus = _serviceProvider.GetService<ClientBusProxy>();
            await eventBus.Connected(item.Id);
        }

        public async Task OnSendAIDetectRequest(string videoId, AIDetectItem item, byte[] pressData, int width, int height)
        {
            var eventBus = _serviceProvider.GetService<ClientBusProxy>();
            await eventBus.PublishAIDetectRequest(videoId, item.Code, item.paramValues, item.EnableDraw, pressData, width, height);
        }

        public async Task OnDeviceDownMessage(BaseDeviceMessage msg)
        {
            if (msg is AIDetectResponseMessage aiResponse)
            {
                ZLMediaKitServer.Instance.UpdateAIDraw(aiResponse.DeviceId, aiResponse.DetType, aiResponse.BoxList);
            }
            else if (msg is MediaItemMessage upItemResponse)
            {
                VideoData videoData = new VideoData();
                videoData.Item = upItemResponse.Item;
                videoData.DetectList = new List<AIDetectorTask>();
                foreach (var it in upItemResponse.Config.Tasks)
                {
                    videoData.DetectList.Add(new AIDetectorTask(it));
                }
                videoData.CoolDownMs = upItemResponse.Config.CoolDownMs;
                videoData.MotionRatio = upItemResponse.Config.MotionRatio;
                ZLMediaKitServer.Instance.AddPullProxy(videoData);
            }
            else if (msg is MediaDelItemMessage delItemResponse)
            {
                ZLMediaKitServer.Instance.RemovePullProxy(delItemResponse.DeviceId);
            }
        }

    }
}
