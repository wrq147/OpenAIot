using Common.EventBus;
using Common.Share;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet.AspNetCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.NetCore;

namespace MqttService
{
    public class PluginConfig : TANetCorePluginConfig
    {
        private ILogger<PluginConfig> _log;
        protected override void ConfigureServices(IConfiguration config, IServiceCollection services)
        {
            var cs = config.GetSection("MqttService");
            services.Configure<MqttOption>(cs);
            var option = cs.Get<MqttOption>();
            if (!option.enable_emqx)
            {
                services.AddSingleton<MqttController>();
                services.AddServices(ac =>
                {
                    ac.AddMqttServer(options =>
                    {
                        options.WithDefaultEndpoint();
                        options.WithDefaultEndpointPort(option.mqtt_tcp_port);
                    });

                    ac.AddMqttTcpServerAdapter();
                    ac.AddMqttWebSocketServerAdapter();
                });
            }
            else
            {
                services.AddSingleton<EmqxController>();
            }
        }
        protected override void Configure(ITAApplication app, PluginObject plg)
        {
            _log = app.ServiceProvider.GetService<ILoggerFactory>().CreateLogger<PluginConfig>();
            var option = app.ServiceProvider.GetService<IOptions<MqttOption>>();
            if (!option.Value.enable_emqx)
            {
                app.ServiceProvider.GetService<MqttController>().Startup();

                TANetCoreHttpApplication httpapp = (TANetCoreHttpApplication)app;
                httpapp.AppBuilder.Use(async (context, next) =>
                {
                    try
                    {
                        if (!context.WebSockets.IsWebSocketRequest || context.Request.Path != "/mqtt")
                        {
                            await next();
                            return;
                        }

                        string subProtocol = null;

                        if (context.Request.Headers.TryGetValue("Sec-WebSocket-Protocol", out var requestedSubProtocolValues))
                        {
                            subProtocol = MqttSubProtocolSelector.SelectSubProtocol(requestedSubProtocolValues);
                        }
                        var adapter = app.ServiceProvider.GetService<MqttWebSocketServerAdapter>();
                        using (var webSocket = await context.WebSockets.AcceptWebSocketAsync(subProtocol).ConfigureAwait(false))
                        {
                            await adapter.RunWebSocketConnectionAsync(webSocket, context);
                        }
                    }
                    catch (Exception ex)
                    {
                        _log.LogError(ex.StackTrace);
                    }
                });
            }


            var generalOption = app.ServiceProvider.GetService<IOptions<GeneralOption>>();
            TAEventDispatcher.Instance.RegisterPluginAllLoad(async (evt) =>
            {
                if (!string.IsNullOrEmpty(generalOption.Value.event_bus_conn))
                {
                    var bus = app.ServiceProvider.GetService<NatsScope>().Bus;
                    string tmpsubid = string.IsNullOrEmpty(option.Value.node_name) ? "Mqtt" : option.Value.node_name;
                    Task t1 = Task.Run(async () =>
                    {
                        await foreach (var msg in bus.SubscribeAsync("MqttNotice.Msg", tmpsubid, DefalutNatsJsonSerializer<List<string>>.Default))
                        {
                            try
                            {
                                if (msg.Data == null)
                                {
                                    continue;
                                }
                                if (!option.Value.enable_emqx)
                                {
                                    await app.ServiceProvider.GetService<MqttController>().NoticeData(msg.Data[0], msg.Data[1]);
                                }
                                else
                                {
                                    await app.ServiceProvider.GetService<EmqxController>().NoticeData(msg.Data[0], msg.Data[1]);
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                        }
                    });

                    Task t2 = Task.Run(async () =>
                    {
                        await foreach (var msg in bus.SubscribeAsync("Mqtt.User.New", tmpsubid, DefalutNatsJsonSerializer<List<string>>.Default))
                        {
                            if (msg.Data == null)
                            {
                                continue;
                            }
                            //发送前端mqtt通知
                            if (!option.Value.enable_emqx)
                            {
                                await app.ServiceProvider.GetService<MqttController>().NoticeUpdate(msg.Data[0], msg.Data[1]);
                            }
                            else
                            {
                                await app.ServiceProvider.GetService<EmqxController>().NoticeUpdate(msg.Data[0], msg.Data[1]);
                            }
                        }
                    });

                }
                else
                {
                    plg.Dispatcher.Register<List<string>>("Mqtt.User.New", async (evt) =>
                    {
                        //发送前端mqtt通知
                        if (!option.Value.enable_emqx)
                        {
                            await app.ServiceProvider.GetService<MqttController>().NoticeUpdate(evt[0], evt[1]);
                        }
                        else
                        {
                            await app.ServiceProvider.GetService<EmqxController>().NoticeUpdate(evt[0], evt[1]);
                        }
                    });
                }
            });


        }

    }
}
