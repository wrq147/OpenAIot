using ChannelUtility;
using ChannelUtility.Message;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnvifChannel
{
    public class OnvifDeviceEventListener
    {
        private IServiceProvider _serviceProvider;
        public OnvifDeviceEventListener(IServiceProvider provider)
        {
            _serviceProvider = provider;
        }
        public void OnSendAIDetectRequest(string videoId, string videoKey, float motionRatio, byte[] pressData, int width, int height, List<AIConfigData> confs, byte dataType)
        {
            try
            {
                var eventBus = _serviceProvider.GetService<ClientBusProxy>();
                eventBus.PublishAIDetectRequest(videoId, videoKey, motionRatio, pressData, width, height, confs, dataType);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
