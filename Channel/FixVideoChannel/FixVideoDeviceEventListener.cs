using ChannelUtility;
using ChannelUtility.Message;
using Microsoft.Extensions.DependencyInjection;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.PixelFormats;
using System;

namespace FixVideoChannel
{
    public class FixVideoDeviceEventListener : IDeviceEventListener
    {
        private IServiceProvider _serviceProvider;
        private FixVideoService _service;
        public FixVideoDeviceEventListener(IServiceProvider provider, FixVideoService service)
        {
            _serviceProvider = provider;
            _service = service;
        }


        public async Task OnEventOffline(VideoCaptureItem item)
        {
            var eventBus = _serviceProvider.GetService<ClientBusProxy>();
            await eventBus.Disconnect(item.Id);
            _service.DelVideo(item.Id);
        }

        public async Task OnEventOnline(VideoCaptureItem item)
        {
            var eventBus = _serviceProvider.GetService<ClientBusProxy>();
            await eventBus.Connected(item.Id);
        }

        public async Task OnSendAIDetectRequest(string videoId, AIDetectItem item, byte[] rgbFrame, int width, int height)
        {
            using (var image = Image.LoadPixelData<Rgb24>(rgbFrame, width, height))
            using (var ms = new MemoryStream())
            {
                // 配置WebP有损压缩参数
                var webpEncoder = new WebpEncoder
                {
                    Method = WebpEncodingMethod.Default
                };

                image.Save(ms, webpEncoder);
                byte[] pressData = ms.ToArray();
                var eventBus = _serviceProvider.GetService<ClientBusProxy>();
                await eventBus.PublishAIDetectRequest(videoId, item.DetectType, item.DetectParams, item.EnableDraw, pressData, width, height);
            }
        }

        public async Task OnDeviceDownMessage(BaseDeviceMessage msg)
        {
            if (msg is AIDetectResponseMessage aiResponse)
            {
                _service.UpdateAIDraw(aiResponse.DeviceId, aiResponse.DetType, aiResponse.BoxList);
            }
            else if (msg is UpVideoItemMessage upItemResponse)
            {
                await _service.VideoCaptureItemEvent(upItemResponse);
            }
            else if (msg is DelVideoItemMessage delItemResponse)
            {
                _service.DelVideo(delItemResponse.DeviceId);
            }
        }

    }
}
