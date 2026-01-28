using ChannelUtility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;


namespace FixVideoChannel
{
    public class FixVideoService : BackgroundService
    {
        private IServiceProvider _provider;
        private IVideoDeviceEventListener _deviceEventListener;
        private FixVideoOption _option;
        public FixVideoService(IServiceProvider provider)
        {
            _provider = provider;
            _option = provider.GetService<IOptions<FixVideoOption>>().Value;
            _deviceEventListener = new FixVideoDeviceEventListener(provider);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var eventBus = _provider.GetService<ClientBusProxy>();
            eventBus.OnSubProductMessage += _deviceEventListener.OnDeviceDownMessage;
            ZLMediaKitServer.Instance.Start(_option, _provider, _deviceEventListener);
        }
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            ZLMediaKitServer.Instance.Stop();
            return base.StopAsync(cancellationToken);
        }
    }

}
