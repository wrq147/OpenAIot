using GB28181.App;
using GB28181.Config;
using GB28181.Servers;
using GB28181.Servers.SIPMessages;
using GB28181.Sys;
using GB28181Channel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SIPSorcery.SIP;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace GB28181.Server.Main
{

    public interface IMainProcess
    {
        void Run();
        void Stop();
    }

    public partial class MainProcess : IMainProcess
    {
        private static readonly ILogger logger = AppState.GetLogger("Process");
        private readonly CancellationTokenSource _processingServiceToken = new CancellationTokenSource();
        private readonly CancellationTokenSource _registryServiceToken = new CancellationTokenSource();
        private ISipMessageCore _mainSipService;
        private MessageHub messageCenter;
        private ISIPRegistrarCore _registry;
        private IServiceProvider _serviceProvider;

        public MainProcess(ISipMessageCore sipMessageCore, MessageHub messageHub, ISIPRegistrarCore sipRegistrarCore, IServiceProvider serviceProvider)
        {
            _mainSipService = sipMessageCore;
            messageCenter = messageHub;
            _registry = sipRegistrarCore;
            _serviceProvider = serviceProvider;
        }

        public void Stop()
        {
            _processingServiceToken.Cancel();
            _registryServiceToken.Cancel();
        }
        private SIPAccount SipAccountStorage_GBServerConfigReceived()
        {
            try
            {
                var option = _serviceProvider.GetService<IOptions<GB28181Option>>().Value;

                SIPAccount obj = new SIPAccount
                {
                    Id = Guid.NewGuid(),
                    GbVersion = "GB-2016",
                    LocalID = option.sip_service_id,
                    LocalIP = IPAddress.Parse("0.0.0.0"),
                    LocalPort = option.sip_listen_port,
                    SIPUsername = string.Empty,
                    SIPPassword = string.Empty,
                    MsgProtocol = System.Net.Sockets.ProtocolType.Udp,
                    StreamProtocol = System.Net.Sockets.ProtocolType.Udp,
                    TcpMode = GB28181.Net.RTP.TcpConnectMode.passive,
                    MsgEncode = "GB2312"
                };
                return obj;
            }
            catch (Exception ex)
            {
                logger.LogWarning("GetIntegratedPlatformConfigRequest: " + ex.Message);
                return null;
            }
        }
        private SIPRequestAuthenticationResult SIPAuthenticateRequestDelegate(SIPEndPoint localSIPEndPoint, SIPEndPoint remoteEndPoint, SIPRequest sipRequest, SIPAccount sipAccount)
        {
            return new SIPRequestAuthenticationResult(true, true);
        }
        public void Run()
        {
            SipStorage.GBServerConfigReceived += SipAccountStorage_GBServerConfigReceived;
            _registry.SetAuthenticateRequestDelegate(SIPAuthenticateRequestDelegate);
            Task.Factory.StartNew(() => Processing(), _processingServiceToken.Token);
        }

        private void Processing()
        {
            try
            {
                Task.Run(() =>
                {
                    _mainSipService.OnKeepaliveReceived += messageCenter.OnKeepaliveReceived;
                    _mainSipService.OnServiceChanged += messageCenter.OnServiceChanged;
                    _mainSipService.OnCatalogReceived += messageCenter.OnCatalogReceived;
                    //_mainSipService.OnNotifyCatalogReceived += messageHandler.OnNotifyCatalogReceived;
                    _mainSipService.OnAlarmReceived += messageCenter.OnAlarmReceived;
                    //_mainSipService.OnRecordInfoReceived += messageHandler.OnRecordInfoReceived;
                    _mainSipService.OnDeviceStatusReceived += messageCenter.OnDeviceStatusReceived;
                    _mainSipService.OnDeviceInfoReceived += messageCenter.OnDeviceInfoReceived;
                    _mainSipService.OnMediaStatusReceived += messageCenter.OnMediaStatusReceived;
                    _mainSipService.OnPresetQueryReceived += messageCenter.OnPresetQueryReceived;
                    _mainSipService.OnDeviceConfigDownloadReceived += messageCenter.OnDeviceConfigDownloadReceived;
                    _mainSipService.OnResponseCodeReceived += messageCenter.OnResponseCodeReceived;
                    _mainSipService.Start();

                });
                Task.Factory.StartNew(() => _registry.ProcessRegisterRequest(), _registryServiceToken.Token);
            }
            catch (Exception exMsg)
            {
                logger.LogError(exMsg.Message);
                throw exMsg;
            }

        }

    }
}
