using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using ChannelUtility;
using ChannelUtility.Config;

namespace MqttChannel
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
                var configSec = configuration.GetSection("MqttOption");
                services.Configure<MqttOption>(configSec);


                services.AddEventBus(x =>
                {
                    var option = configSec.Get<MqttOption>();
                    x.NodeId = option.node_id;
                    x.EventConn = option.event_conn;
                    x.EventUser = option.event_user;
                    x.EventPass = option.event_pass;
                    x.RedisConn = option.redis_conn;
                    ChannelConfig config = new ChannelConfig();
                    config.Name = "MqttModbus接入";
                    config.Code = "mqtt_modbus";
                    config.Remark = "设备通过Mqtt接入平台";
                    config.CanScript = true;
                    config.CanModbus = true;
                    config.CanBind = true;
                    config.CanModify = false;
                    x.config = config;
                });
                services.AddHostedService<MQTTService>();

            }).ConfigureLogging(loggingBuilder =>
            {
                loggingBuilder.AddConfiguration(configuration.GetSection("Logging"));
                loggingBuilder.AddConsole();
                loggingBuilder.AddDebug();
            });
        }
    }
}
