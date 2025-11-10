using FastTunnel.Core.Client;
using FastTunnel.Core.Config;
using FastTunnel.Core.Extensions;
using FastTunnel.Core.Handlers.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading;

namespace ModbusChannel
{
    public class CustomFastTunnelClient : FastTunnelClient
    {
        private string _machineId;
        private string _md5seckey;
        public CustomFastTunnelClient(
      string machineId,
      string md5devsec,
      ILogger<FastTunnelClient> logger,
      SwapHandler newCustomerHandler,
      LogHandler logHandler,
      DefaultClientConfig config)
        : base(logger, newCustomerHandler, logHandler, config)
        {
            _machineId = machineId;
            _md5seckey = md5devsec;
        }

        protected override string GetLoginMsg(CancellationToken cancellationToken)
        {
            return new LogInByKeyMassage { key = _machineId, md5devkey = _md5seckey, webport = 880 }.ToJson(); ;
        }
    }
}
