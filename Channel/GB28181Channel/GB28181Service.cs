using ChannelUtility;
using DnsClient.Internal;
using LibGB28181SipServer;
using LibGB28181SipServer.Structs;
using LibGB28181SipServer.Structs.GB28181;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GB28181Channel
{
    public class GB28181Service : BackgroundService
    {
        private MqttFactory _mqttFactory = new MqttFactory();
        private IMqttClient _client;
        private IServiceProvider _provider;
        private GB28181DeviceEventListener _deviceEventListener;
        private SipServer _server;
        private GB28181Option _option;
        public GB28181Service(IServiceProvider provider)
        {
            _provider = provider;
            _deviceEventListener = new GB28181DeviceEventListener(provider);
            var logger = _provider.GetService<Microsoft.Extensions.Logging.ILoggerFactory>().CreateLogger<SipServer>();

            _option = provider.GetService<IOptions<GB28181Option>>().Value;
            var sipServerConfig = new SipServerConfig();
            sipServerConfig.Authentication = true;

            sipServerConfig.GbVersion = "GB-2016";
            sipServerConfig.MsgProtocol = "TCP"; //使用TCP可以完美支持tcp信令
            sipServerConfig.SipPort = (ushort)_option.sip_listen_port;
            sipServerConfig.IpV6Enable = _option.ipv6_enable;
            if (sipServerConfig.IpV6Enable)
            {
                sipServerConfig.SipIpV6Address = "0::0";
            }

            sipServerConfig.SipIpAddress = "0.0.0.0";
            sipServerConfig.KeepAliveInterval = 5;
            sipServerConfig.KeepAliveLostNumber = 3;
            /*SipDeviceID 20位编码规则
             *1-2省级 33 浙江省
             *3-4市级 02 宁波市
             *5-6区级 00 宁波市区
             *7-8村级 00 宁波市区
             *9-10行业 02 社会治安内部接入
             *11-13设备类型 118 NVR
             *14 网络类型 0 监控专用网
             *15-20 设备序号 000001 1号设备
             */
            sipServerConfig.ServerSipDeviceId = _option.sip_service_id;
            sipServerConfig.Realm = sipServerConfig.ServerSipDeviceId.Substring(0, 10);
            _server = new SipServer(logger, sipServerConfig);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var eventBus = _provider.GetService<ClientBusProxy>();
            eventBus.OnSubProductMessage += _deviceEventListener.OnDeviceDownMessage;
            _server.Start();
        }
    }
}
