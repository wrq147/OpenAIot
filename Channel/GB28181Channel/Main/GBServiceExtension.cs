using ChannelUtility;
using GB28181;
using GB28181.Cache;
using GB28181.Config;
using GB28181.Server.Main;
using GB28181.Servers;
using GB28181.Servers.SIPMessages;
using GB28181.Servers.SIPMonitor;
using GB28181.Sys.Model;
using Microsoft.Extensions.DependencyInjection;

namespace GB28181Channel.Main
{
    public static class GBServiceExtension
    {
        public static void AddGB28281(this IServiceCollection services)
        {
            services.AddSingleton<ISipStorage, SipStorage>()
             .AddSingleton<MessageHub>()
             .AddScoped<ISIPServiceDirector, SIPServiceDirector>()
             .AddTransient<ISIPMonitorCore, SIPMonitorCore>()
             .AddSingleton<ISipMessageCore, SIPMessageCore>()
             .AddSingleton<ISIPTransport, SIPTransport>()
             .AddTransient<ISIPTransactionEngine, SIPTransactionEngine>()
             .AddSingleton<ISIPRegistrarCore, SIPRegistrarCore>()
             .AddSingleton<IMemoCache<Camera>, DeviceObjectCache>()
             .AddSingleton<IMainProcess, MainProcess>();

        }
    }
}
