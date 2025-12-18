using ChannelUtility;
using ChannelUtility.Message;
using Microsoft.Extensions.DependencyInjection;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixVideoChannel
{
    public class FixVideoDeviceEventListener : IDeviceEventListener
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
        }

        public async Task OnEventOnline(VideoCaptureItem item)
        {
            var eventBus = _serviceProvider.GetService<ClientBusProxy>();
            await eventBus.Connected(item.Id);
        }

        public async Task OnSendAIDetectRequest(VideoCaptureItem item, string detectType, Dictionary<string, string> detectParams, byte[] rgbFrame, int width, int height)
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
                await eventBus.PublishAIDetectRequest(item.Id, detectType, detectParams, pressData, width, height);
            }
        }

        public Task OnDeviceDownMessage(RequestMessage msg)
        {
        
        }

    }
}
