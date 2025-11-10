using ChannelUtility.Redis;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ChannelUtility
{
    public static class ChannelServiceExtends
    {
        public static void AddEventBus(this IServiceCollection services, Action<ChannelOption> setupAction)
        {
            ChannelOption option = new ChannelOption();
            setupAction.Invoke(option);
            services.AddSingleton(option);
            services.AddSingleton<GeneralRedisHelper>();
            services.AddSingleton<ClientBusProxy>();
            services.AddMemoryCache();
        }
    }
}
