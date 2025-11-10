using ChannelUtility;
using Microsoft.Extensions.Options;

namespace HttpChannel.Services
{
    public class HttpService:BackgroundService
    {
        private IServiceProvider _provider;
        private ClientBusProxy _eventBus;
       
        public HttpService(IServiceProvider provider)
        {
            _provider = provider;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var option = _provider.GetService<IOptions<HttpChannelOption>>();       
            byte[] buffer = null;
            Start(buffer);
        }

        private void Start(byte[] data){
             _eventBus = _provider.GetService<ClientBusProxy>();

            _eventBus.OnSubProductMessage += async (msg, ret) =>
            {
                await _eventBus.ConfirmBindReply("ProductId", "DeviceId", true, "MessageId");
            };
        }
    }
}
