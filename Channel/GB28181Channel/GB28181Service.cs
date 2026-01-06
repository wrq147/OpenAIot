using ChannelUtility;
using GB28181.Server.Main;
using GB28181.Sys;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GB28181Channel
{
    public class GB28181Service : BackgroundService
    {
        private IServiceProvider _provider;
        private GB28181DeviceEventListener _deviceEventListener;
        private IMainProcess _server;
        private GB28181Option _option;
        public GB28181Service(IServiceProvider provider)
        {
            _provider = provider;
            AppState.InitAppState(_provider);
            _deviceEventListener = new GB28181DeviceEventListener(provider);
            _option = provider.GetService<IOptions<GB28181Option>>().Value;
            _server = provider.GetService<IMainProcess>();

        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var eventBus = _provider.GetService<ClientBusProxy>();
            eventBus.OnSubProductMessage += _deviceEventListener.OnDeviceDownMessage;
            _server.Run();
        }
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _server.Stop();
            return base.StopAsync(cancellationToken);
        }
    }
}
