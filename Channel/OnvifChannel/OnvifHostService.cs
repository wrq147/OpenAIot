using ChannelUtility;
using ChannelUtility.Message;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic.FileIO;
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
        private OnvifDeviceEventListener _eventListener;
        private OnvifOption _option;
        public OnvifHostService(IServiceProvider provider)
        {
            _provider = provider;
            _option = provider.GetService<IOptions<OnvifOption>>().Value;
            _eventListener = new OnvifDeviceEventListener(_provider);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var eventBus = _provider.GetService<ClientBusProxy>();
            eventBus.OnSubProductMessage += _eventListener.OnDeviceDownMessage;
            ZLMediaKitServer.Instance.Start(_option, _provider, _eventListener);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            ZLMediaKitServer.Instance.Stop();
            return base.StopAsync(cancellationToken);
        }
    }
}
