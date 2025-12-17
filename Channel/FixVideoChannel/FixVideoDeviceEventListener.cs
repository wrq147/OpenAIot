using ChannelUtility;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
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
            using (var memoryStream = new MemoryStream())
            {
                using (var gzipStream = new GZipStream(memoryStream, CompressionLevel.Optimal, leaveOpen: true))
                {
                    await gzipStream.WriteAsync(rgbFrame, 0, rgbFrame.Length);
                }
                byte[] pressData = memoryStream.ToArray();
                var eventBus = _serviceProvider.GetService<ClientBusProxy>();
                await eventBus.PublishAIDetectRequest(item.Id, detectType, detectParams, pressData, width, height);
            }
        }
    }
}
