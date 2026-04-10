using ChannelUtility;
using ChannelUtility.Message;
using GB28181Channel.GB28181;
using GB28181Channel.GB28181.Enum;
using GB28181Channel.GB28181.Interface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GB28181Channel
{
    public class GB28181HostService : BackgroundService
    {
        private IServiceProvider _provider;
        private GB28181DeviceEventListener _deviceEventListener;
        private GB28181Server _server;
        private GB28181Option _option;
        private IDeviceStorage _storage;

        private readonly TimeSpan _redisWriteInterval = TimeSpan.FromSeconds(60);
        private readonly string _redisKey = "GB28181Servers:List";
        private Timer _redisTimer;

        public GB28181HostService(IServiceProvider provider, IDeviceStorage storage)
        {
            _provider = provider;
            _option = provider.GetService<IOptions<GB28181Option>>().Value;
            _deviceEventListener = new GB28181DeviceEventListener(_provider);
            _storage = storage;
        }
        private async Task OnDeviceDownMessageHandler(BaseDeviceMessage msg)
        {
            await _deviceEventListener.OnDeviceDownMessage(msg, _server);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var eventBus = _provider.GetService<ClientBusProxy>();
            _redisTimer = new Timer(callback: WriteRedisHeartbeat, state: null, dueTime: TimeSpan.Zero, period: _redisWriteInterval);

            eventBus.OnSubProductMessage += OnDeviceDownMessageHandler;
            SIPTransportProtocol protocol = SIPTransportProtocol.Both;
            if (_option.sip_protocol == "udp")
            {
                protocol = SIPTransportProtocol.UdpOnly;
            }
            else if (_option.sip_protocol == "tcp")
            {
                protocol = SIPTransportProtocol.TcpOnly;
            }
            _server = new GB28181Server(_option.sip_ip, _option.sip_port, _option.sip_service_id, _storage, protocol);
            _server.DeviceRegistered += _deviceEventListener.OnDeviceRegistered;
            _server.DeviceOffline += _deviceEventListener.OnDeviceOffline;
            _server.StreamPlayed += _deviceEventListener.OnStreamPlay;
            _server.PresetListReceived += _deviceEventListener.OnPresetListReceived;
            _server.PTZEventOk += _deviceEventListener.OnPTZEventOk;
            _server.Start();

            ZLMediaKitServer.Instance.Start(_option, _provider, _deviceEventListener, _server);
        }
        private void WriteRedisHeartbeat(object state)
        {
            try
            {
                var eventBus = _provider.GetService<ClientBusProxy>();
                eventBus.RedisHelper.HashSet(_redisKey, eventBus.NodeId, new NodeItem()
                {
                    NodeId = eventBus.NodeId,
                    Ip = _option.sip_ip,
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
            _server.Dispose();
            _server = null;
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
