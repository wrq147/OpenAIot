using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedirectMqtt
{
    public class RedirectService : BackgroundService
    {
        private IServiceProvider _provider;
        private List<RedirectTask> _tasks;
        private MqttFactory _mqttFactory = new MqttFactory();
        private ILogger<RedirectService> _log;
        public RedirectService(IServiceProvider provider)
        {
            _tasks = new List<RedirectTask>();
            _provider = provider;
            _log = _provider.GetService<ILoggerFactory>().CreateLogger<RedirectService>();
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var option = _provider.GetService<IOptions<RedirectOption>>();
            foreach (var item in option.Value.DtuIds)
            {
                RedirectTask t = new RedirectTask(_provider, _mqttFactory, _log, item);
                await t.Start(stoppingToken);
            }
        }
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            foreach (var task in _tasks)
            {
                await task.Stop();
            }
        }
    }
}
