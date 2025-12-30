using ChannelUtility;
using ChannelUtility.Message;
using Microsoft.Extensions.DependencyInjection;

namespace FixVideoChannel
{
    public class FixVideoDeviceEventListener
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
            await eventBus.PublishAIDetectRequest(videoId, item.DetectType, item.DetectParams, item.EnableDraw, pressData, width, height);
        }

        public async Task OnDeviceDownMessage(BaseDeviceMessage msg)
        {
            if (msg is AIDetectResponseMessage aiResponse)
            {
                ZLMediaKitServer.Instance.UpdateAIDraw(aiResponse.DeviceId, aiResponse.DetType, aiResponse.BoxList);
            }
            else if (msg is UpVideoItemMessage upItemResponse)
            {
                List<AIDetectorTask> tasks = new List<AIDetectorTask>();
                foreach (var it in upItemResponse.DetectList)
                {
                    tasks.Add(new AIDetectorTask(it));
                }
                ZLMediaKitServer.Instance.AddPullProxy(new VideoData()
                {
                    Item = upItemResponse.Item,
                    DetectList = tasks
                });
            }
            else if (msg is DelVideoItemMessage delItemResponse)
            {
                ZLMediaKitServer.Instance.RemovePullProxy(delItemResponse.DeviceId);
            }
        }

    }
}
