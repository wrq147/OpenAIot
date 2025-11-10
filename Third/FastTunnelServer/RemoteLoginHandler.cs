using FastTunnel.Core.Handlers.Server;
using FastTunnel.Core.Models;
using FastTunnel.Core.Models.Massage;
using Yarp.ReverseProxy.Configuration;
namespace FastTunnelServer
{
    public class RemoteLoginHandler : LoginHandler
    {
        private IServiceProvider _provider;
        public RemoteLoginHandler(ILogger<LoginHandler> logger, IProxyConfigProvider proxyConfig, IServiceProvider provider)
            : base(logger, proxyConfig)
        {
            _provider = provider;
        }

        public override async Task<bool> HandlerMsg(FastTunnel.Core.Client.FastTunnelServer server, TunnelClient client, string content, CancellationToken cancellationToken)
        {
            var logMsg = System.Text.Json.JsonSerializer.Deserialize<LogInByKeyMassage>(content);
            var sql = _provider.GetService<SqlRespository>();
            var device = await sql.InfoByDtuId(logMsg.key);
            if (device != null)
            {
                var developer = await sql.InfoByOrg(device.OrgId.Value);
                var comparekey = MyAccess.Core.Crypter.MD5(developer.SecKey).ToLower();
                Console.WriteLine($"客户端{logMsg.key}代理服务已连接！");
                if (logMsg.md5devkey.ToLower() != comparekey)
                {
                    throw new Exception("无权限远程控制该设备");
                }

                LogInMassage newLogMsg = new LogInMassage();
                var webs = new List<WebConfig>();
                webs.Add(new WebConfig
                {
                    LocalIp = "127.0.0.1",
                    LocalPort = logMsg.webport,
                    SubDomain = logMsg.key,
                });
                webs.Add(new WebConfig
                {
                    LocalIp = "127.0.0.1",
                    LocalPort = 8899,
                    SubDomain = 8899 + "-" + logMsg.key,
                });

                newLogMsg.Webs = webs;
                var forwards = new List<ForwardConfig>();
                newLogMsg.Forwards = forwards;
                await HandleLoginAsync(server, client, newLogMsg, cancellationToken);

                return NeedRecive;
            }
            else
            {
                throw new Exception("远控设备不存在");
            }

        }
    }
}
