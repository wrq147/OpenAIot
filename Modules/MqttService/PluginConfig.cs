using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.NetCore;
using MQTTnet.AspNetCore;
using MQTTnet.Adapter;
using MQTTnet.Implementations;
using MQTTnet.Diagnostics;
using Microsoft.Extensions.Hosting;
using MQTTnet.Server;
using Microsoft.AspNetCore.Builder;
using Common.Share;
using Common.EventBus;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

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
                services.AddSingleton<MqttServerOptions>((object[] constructorArguments, ITAServiceProvider provider) =>
                {
                    var serverOptionsBuilder = new MqttServerOptionsBuilder();
                    serverOptionsBuilder.WithDefaultEndpoint();
                    serverOptionsBuilder.WithDefaultEndpointPort(option.mqtt_tcp_port);
                    return serverOptionsBuilder.Build();
                });

                services.AddSingleton<IMqttNetLogger>(new MqttNetEventLogger());
                services.AddSingleton<MqttHostedServer>();
                services.AddSingleton<IHostedService>((object[] constructorArguments, ITAServiceProvider provider) => provider.GetService<MqttHostedServer>());
                services.AddSingleton<MqttServer>((object[] constructorArguments, ITAServiceProvider provider) => provider.GetService<MqttHostedServer>());

                services.AddSingleton<MqttConnectionHandler>();
                services.AddSingleton<IMqttServerAdapter>((object[] constructorArguments, ITAServiceProvider provider) => provider.GetService<MqttConnectionHandler>());

                //添加MqttTcpServerAdapter
                services.AddSingleton<MqttTcpServerAdapter>();
                services.AddSingleton<IMqttServerAdapter>((object[] constructorArguments, ITAServiceProvider provider) =>
                {
                    return provider.GetService<MqttTcpServerAdapter>();
                });
                //添加MqttWebSocketServerAdapter
                services.AddSingleton<MqttWebSocketServerAdapter>();
                services.AddSingleton<IMqttServerAdapter>((object[] constructorArguments, ITAServiceProvider provider) =>
                {
                    return provider.GetService<MqttWebSocketServerAdapter>();
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
                        await foreach (var msg in bus.SubscribeAsync("/MqttNotice.Msg", tmpsubid, DefalutNatsJsonSerializer<List<string>>.Default))
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
                    });

                    Task t2 = Task.Run(async () =>
                    {
                        await foreach (var msg in bus.SubscribeAsync("/Mqtt.User.New", tmpsubid, DefalutNatsJsonSerializer<string>.Default))
                        {
                            if (msg.Data == null)
                            {
                                continue;
                            }
                            //发送前端mqtt通知
                            if (!option.Value.enable_emqx)
                            {
                                await app.ServiceProvider.GetService<MqttController>().NoticeUpdate(msg.Data);
                            }
                            else
                            {
                                await app.ServiceProvider.GetService<EmqxController>().NoticeUpdate(msg.Data);
                            }
                        }
                    });

                }
                else
                {
                    plg.Dispatcher.Register<string>("/Mqtt.User.New", async (evt) =>
                    {
                        //发送前端mqtt通知
                        if (!option.Value.enable_emqx)
                        {
                            await app.ServiceProvider.GetService<MqttController>().NoticeUpdate(evt);
                        }
                        else
                        {
                            await app.ServiceProvider.GetService<EmqxController>().NoticeUpdate(evt);
                        }
                    });
                }
            });


        }

    }
}
