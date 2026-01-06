using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MqttChannel
{
    public class MQTTService : BackgroundService
    {
        private IServiceProvider _provider;
        private IController _controller;
        public MQTTService(IServiceProvider provider)
        {
            _provider = provider;
            var option = _provider.GetService<IOptions<MqttOption>>();
            if (string.IsNullOrEmpty(option.Value.mqtt_server))
            {
                _controller = new MqttController(provider);
            }
            else
            {
                _controller = new EmqxController(provider);
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _controller.ExecuteAsync(stoppingToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _controller.StopAsync(cancellationToken);
        }
    }
}
