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

        public async Task OnSendAIDetectRequest(string videoId, string videoKey, float motionRatio, byte[] pressData, int width, int height, List<AIConfigData> confs)
        {
            var eventBus = _serviceProvider.GetService<ClientBusProxy>();
            await eventBus.PublishAIDetectRequest(videoId, videoKey, motionRatio, pressData, width, height, confs);
        }

        public async Task OnDeviceDownMessage(BaseDeviceMessage msg)
        {
            if (msg is AIDetectResponseMessage aiResponse)
            {
                ZLMediaKitServer.Instance.UpdateAIDraw(aiResponse.DeviceId, aiResponse.BoxList, aiResponse.NeedConf);
            }
            else if (msg is MediaItemMessage upItemResponse)
            {
                VideoData videoData = new VideoData();
                videoData.NeedUp = true;
                videoData.Item = upItemResponse.Item;
                videoData.Configs = new List<AIConfigData>();
                videoData.NeedUp = true;
                foreach (var it in upItemResponse.Config.Tasks)
                {
                    videoData.Configs.Add(new AIConfigData()
                    {
                        DetType = it.Code,
                        IsDraw = it.EnableDraw,
                        DetParams = it.paramValues
                    });
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
