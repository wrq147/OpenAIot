using ChannelUtility;
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
        private IMediaHandler _mediaHandler;
        public GB28181HostService(IServiceProvider provider, IDeviceStorage storage, IMediaHandler mediaHandler)
        {
            _provider = provider;
            _option = provider.GetService<IOptions<GB28181Option>>().Value;
            _deviceEventListener = new GB28181DeviceEventListener(_provider);
            _storage = storage;
            _mediaHandler = mediaHandler;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var eventBus = _provider.GetService<ClientBusProxy>();
            eventBus.OnSubProductMessage += _deviceEventListener.OnDeviceDownMessage;
            SIPTransportProtocol protocol = SIPTransportProtocol.Both;
            if (_option.sip_protocol == "udp")
            {
                protocol = SIPTransportProtocol.UdpOnly;
            }
            else if (_option.sip_protocol == "tcp")
            {
                protocol = SIPTransportProtocol.TcpOnly;
            }
            _server = new GB28181Server(_option.sip_ip, _option.sip_port, _option.sip_service_id, GB28181Version.V2016, _storage, _mediaHandler, protocol);
            _server.DeviceRegistered += _deviceEventListener.OnDeviceRegistered;
            _server.DeviceOffline += _deviceEventListener.OnDeviceOffline;
            _server.Start();
        }



        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _server.Dispose();
            _server = null;
            return base.StopAsync(cancellationToken);
        }
    }
}
