using ChannelUtility;
using ChannelUtility.Message;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnvifChannel
{
    public class OnvifHostService : BackgroundService
    {
        private IServiceProvider _provider;
        public OnvifHostService(IServiceProvider provider)
        {
            _provider = provider;
        }
        private async Task OnDeviceDownMessageHandler(BaseDeviceMessage msg)
        {
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var eventBus = _provider.GetService<ClientBusProxy>();
            eventBus.OnSubProductMessage += OnDeviceDownMessageHandler;
        
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {

            return base.StopAsync(cancellationToken);
        }
    }
}
