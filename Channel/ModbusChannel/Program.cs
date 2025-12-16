using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using ChannelUtility;
using FastTunnel.Core;
using FastTunnel.Core.Client;
using FastTunnel.Core.Extensions;
using FastTunnel.Core.Handlers.Client;
using FastTunnel.Core.Services;
using FastTunnel.Core.Client.Extensions;
using FastTunnel.Core.Config;


namespace ModbusChannel
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
                var configSec = configuration.GetSection("ModbusOption");
                services.Configure<ModbusOption>(configSec);
                services.AddEventBus(x =>
                {
                    var option = configSec.Get<ModbusOption>();
                    x.EventConn = option.event_conn;
                    x.RedisConn = option.redis_conn;
                    x.config = new ChannelUtility.Config.ChannelConfig();
                    x.config.Name = "Modbus直连接入";
                    x.config.Code = "modbus_only";
                    x.config.Remark = "边缘专用，modbus用的接入方式。";
                    x.config.CanScript = false;
                    x.config.CanModbus = true;
                    x.config.CanBind = false;
                    x.config.CanModify = true;
                });
                services.AddHostedService<ModbusService>();


                services.AddSingleton<LogHandler>().AddSingleton<SwapHandler>();


            }).ConfigureLogging(loggingBuilder => {
                loggingBuilder.AddConfiguration(configuration.GetSection("Logging"));
                loggingBuilder.AddConsole();
                loggingBuilder.AddDebug();
            });
        }
    }
}
