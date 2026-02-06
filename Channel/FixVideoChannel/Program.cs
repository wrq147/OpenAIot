
using ChannelUtility;
using ChannelUtility.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Runtime.InteropServices;

namespace FixVideoChannel
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
                var configSec = configuration.GetSection("FixVideoOption");
                services.Configure<FixVideoOption>(configSec);
                services.AddSingleton<MinioHelper>();
                services.AddEventBus(x =>
                {
                    var option = configSec.Get<FixVideoOption>();
                    x.NodeId = option.node_id;
                    x.EnableAI = true;
                    x.EventConn = option.event_conn;
                    x.EventUser = option.event_user;
                    x.EventPass = option.event_pass;
                    x.RedisConn = option.redis_conn;
                    ChannelConfig config = new ChannelConfig();
                    config.Name = "固定拉流接入";
                    config.Code = "fixedaddr";
                    config.Remark = "监控设备通过固定拉流地址接入平台";
                    config.CanScript = false;
                    config.CanModbus = false;
                    config.CanBind = false;
                    config.CanModify = false;
                    x.config = config;
                });
                services.AddHostedService<FixVideoService>();

            }).ConfigureLogging(loggingBuilder =>
            {
                loggingBuilder.AddConfiguration(configuration.GetSection("Logging"));
                loggingBuilder.AddConsole();
                loggingBuilder.AddDebug();
            });
        }
    }
}
