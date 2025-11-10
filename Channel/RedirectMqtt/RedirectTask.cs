using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;
using System;

namespace RedirectMqtt
{
    public class RedirectTask
    {
        private IServiceProvider _provider;
        private string _dtuId;
        private Task _mainTask;
        private bool _isAborted;
        private MqttFactory _mqttFactory;
        private IMqttClient _sourceclient;
        private IMqttClient _destclient;
        private ILogger<RedirectService> _logger;
        public RedirectTask(IServiceProvider provider, MqttFactory factory, ILogger<RedirectService> logger, string dtuId)
        {
            _provider = provider;
            _dtuId = dtuId;
            _mqttFactory = factory;
            _logger = logger;
        }

        public async Task Start(CancellationToken stoppingToken)
        {
            _sourceclient = _mqttFactory.CreateMqttClient();
            _sourceclient.ApplicationMessageReceivedAsync += Source_MqttServer_ApplicationMessageReceived;
            var option = _provider.GetService<IOptions<RedirectOption>>();
            _mainTask = Task.Run(async () =>
            {
                while (!_isAborted)
                {
                    try
                    {
                        var sourceMqttClientOptions = new MqttClientOptionsBuilder()
        .WithTcpServer(option.Value.Source_Server, option.Value.Source_Port)
        .WithClientId(Guid.NewGuid().ToString("N"))
        .WithCredentials(option.Value.Source_UserName, option.Value.Source_Password)
        .Build();

                        //断开重连
                        _sourceclient.DisconnectedAsync += async e =>
                        {
                            await Task.Delay(TimeSpan.FromSeconds(10));

                            try
                            {
                                await ConnectMqtt(_sourceclient, sourceMqttClientOptions, stoppingToken);
                            }
                            catch (Exception ex)
                            {
                                // Handle reconnect exception
                                Console.WriteLine($"Exception during reconnect: {ex.Message}");
                            }
                        };

                        await ConnectMqtt(_sourceclient, sourceMqttClientOptions, stoppingToken);
                    }
                    catch (TaskCanceledException)
                    {
                        _isAborted = true;
                    }
                    catch (ThreadAbortException)
                    {
                        _isAborted = true;
                    }
                    catch (ThreadInterruptedException)
                    {
                        _isAborted = true;
                    }
                    catch { }
                }
            });
        }
        private async Task Source_MqttServer_ApplicationMessageReceived(MqttApplicationMessageReceivedEventArgs e)
        {
            try
            {
                var option = _provider.GetService<IOptions<RedirectOption>>();
                if (e.ApplicationMessage.Topic.StartsWith("$SYS/brokers"))
                {
                    if (e.ApplicationMessage.Topic.EndsWith("/disconnected"))
                    {
                        await _destclient.DisconnectAsync();
                        _destclient = null;
                        _sourceclient = null;
                    }
                    else if (e.ApplicationMessage.Topic.EndsWith("/connected"))
                    {
                        _destclient = _mqttFactory.CreateMqttClient();
                        _destclient.ApplicationMessageReceivedAsync += Dest_MqttServer_ApplicationMessageReceived;
                        var destMqttClientOptions = new MqttClientOptionsBuilder()
                            .WithTcpServer(option.Value.Dest_Server, option.Value.Dest_Port)
                            .WithClientId(Guid.NewGuid().ToString("N"))
                            .WithCredentials(option.Value.Dest_UserName, option.Value.Dest_Password)
                            .Build();
                        await _destclient.ConnectAsync(destMqttClientOptions);
                    }
                }
                else
                {
                    if (_destclient != null)
                    {
                        await _destclient.PublishAsync(e.ApplicationMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "\n" + ex.StackTrace);
            }

        }
        private async Task Dest_MqttServer_ApplicationMessageReceived(MqttApplicationMessageReceivedEventArgs e)
        {
            if (_sourceclient != null)
            {
                await _sourceclient.PublishAsync(e.ApplicationMessage);
            }
        }
        private async Task ConnectMqtt(IMqttClient client, MqttClientOptions option, CancellationToken stoppingToken)
        {
            await client.ConnectAsync(option);

            //订阅离在线
            var mqttConnOption = _mqttFactory.CreateSubscribeOptionsBuilder().WithTopicFilter(f =>
            {
                f.WithTopic("$share/ts/$SYS/brokers/+/clients/" + _dtuId+ "/+");
            }).Build();
            await client.SubscribeAsync(mqttConnOption, stoppingToken);


            //订阅设备消息
            var mqttRecvOption = _mqttFactory.CreateSubscribeOptionsBuilder().WithTopicFilter(f =>
            {
                f.WithTopic("$share/ts/wukong/up/" + _dtuId);
            }).Build();
            await client.SubscribeAsync(mqttRecvOption, stoppingToken);
        }
        public async Task Stop()
        {
            _isAborted = true;
        }
    }
}
