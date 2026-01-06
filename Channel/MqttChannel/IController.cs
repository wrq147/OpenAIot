using System;
using System.Threading;
using System.Threading.Tasks;

namespace MqttChannel
{
    public interface IController
    {
        Task ExecuteAsync(CancellationToken stoppingToken);
        Task StopAsync(CancellationToken cancellationToken);
    }
}
