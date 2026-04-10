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

        private readonly TimeSpan _redisWriteInterval = TimeSpan.FromSeconds(60);
        private readonly string _redisKey = "OnvifServers:List";
        private Timer _redisTimer;
        public OnvifHostService(IServiceProvider provider)
        {
            _provider = provider;
            _option = provider.GetService<IOptions<OnvifOption>>().Value;
            _eventListener = new OnvifDeviceEventListener(_provider);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var eventBus = _provider.GetService<ClientBusProxy>();

            _redisTimer = new Timer(callback: WriteRedisHeartbeat, state: null, dueTime: TimeSpan.Zero, period: _redisWriteInterval);

            eventBus.OnSubProductMessage += _eventListener.OnDeviceDownMessage;
            ZLMediaKitServer.Instance.Start(_option, _provider, _eventListener);
        }
        private void WriteRedisHeartbeat(object state)
        {
            try
            {
                var eventBus = _provider.GetService<ClientBusProxy>();
                eventBus.RedisHelper.HashSet(_redisKey, eventBus.NodeId, new NodeItem()
                {
                    NodeId = eventBus.NodeId,
                    Ip = _option.server_ip,
                    RtmpPort = _option.rtmp_port,
                    HttpPort = _option.http_port,
                    Expire = DateTime.Now.AddSeconds(360)
                });
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] 定时写入Redis Key成功：{_redisKey}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Redis写入失败：{ex.Message}");
            }
        }
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            ZLMediaKitServer.Instance.Stop();
            return base.StopAsync(cancellationToken);
        }

        public class NodeItem
        {
            public string NodeId { get; set; }
            public string Ip { get; set; }
            public int RtmpPort { get; set; }
            public int HttpPort { get; set; }
            public DateTime Expire { get; set; }
        }
    }
}
