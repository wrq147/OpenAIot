
using ChannelUtility;
using ChannelUtility.Config;
using GB28181Channel.GB28181.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Runtime.InteropServices;

namespace GB28181Channel
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args);
            //判断当前系统是否为windows
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                host.UseWindowsService();
            }
            var configuration = new ConfigurationBuilder()
              .SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("appsettings.json", false)
              .Build();

            return host.ConfigureServices((hostContext, services) =>
            {
                var configSec = configuration.GetSection("GB28181Option");
                services.Configure<GB28181Option>(configSec);
                services.AddSingleton<IDeviceStorage, InMemoryDeviceStorage>();
                services.AddEventBus(x =>
                {
                    var option = configSec.Get<GB28181Option>();
                    x.NodeId = option.sip_service_id;
                    x.EventConn = option.event_conn;
                    x.EventUser = option.event_user;
                    x.EventPass = option.event_pass;
                    x.RedisConn = option.redis_conn;
                    ChannelConfig config = new ChannelConfig();
                    config.Name = "GB28181接入";
                    config.Code = "gb28181";
                    config.Remark = "监控设备通过GB28181协议接入平台";
                    config.CanScript = true;
                    config.CanModbus = false;
                    config.CanBind = false;
                    config.CanModify = false;
                    x.config = config;
                });
                services.AddHostedService<GB28181HostService>();

            }).ConfigureLogging(loggingBuilder =>
            {
                loggingBuilder.AddConfiguration(configuration.GetSection("Logging"));
                loggingBuilder.AddConsole();
                loggingBuilder.AddDebug();
            });
        }
    }
}
