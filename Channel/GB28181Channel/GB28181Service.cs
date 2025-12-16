using ChannelUtility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MQTTnet;
using MQTTnet.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GB28181Channel
{
    public class GB28181Service : BackgroundService
    {
        private MqttFactory _mqttFactory = new MqttFactory();
        private IMqttClient _client;
        private IServiceProvider _provider;
        public GB28181Service(IServiceProvider provider)
        {
            _provider = provider;
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var eventBus = _provider.GetService<ClientBusProxy>();

        
        }
    }
}
