using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using MQTTnet;
using MQTTnet.Client;
using System;
using System.Threading;
using System.Threading.Tasks;
using ChannelUtility;
using ChannelUtility.Buffers;
using Microsoft.Extensions.Logging;
using ChannelUtility.Message;
using System.Text;
using MqttChannel.Timer;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using EasyNetQ;

namespace MqttChannel
{
    public class MQTTService : BackgroundService
    {
        private IMqttClient _client;
        private IServiceProvider _provider;
        private MqttFactory _mqttFactory = new MqttFactory();
        private ILogger<MQTTService> _log;
        private DownWheelRuner _downRuner;
        private const string WUK_STR = "wukong/up/";
        public MQTTService(IServiceProvider provider)
        {
            _provider = provider;
            _log = _provider.GetService<ILoggerFactory>().CreateLogger<MQTTService>();

        }

        private async Task MqttServer_ApplicationMessageReceived(MqttApplicationMessageReceivedEventArgs e)
        {
            try
            {
                if (e.ApplicationMessage.Topic.StartsWith("$SYS/brokers"))
                {
                    if (e.ApplicationMessage.Topic.EndsWith("/disconnected"))
                    {
                        //处理离线
                        var eventBus = _provider.GetService<ClientBusProxy>();
                        int startIdx = e.ApplicationMessage.Topic.IndexOf("clients/") + 8;
                        int newlen = e.ApplicationMessage.Topic.Length - startIdx - "/disconnected".Length;
                        string clientId = e.ApplicationMessage.Topic.Substring(startIdx, newlen);
                        await eventBus.Disconnect(clientId);

                        _downWaitQueue.TryRemove(clientId, out List<RawDataMessage> requests);
                        _downLastTime.TryRemove(clientId, out DateTime tmpdt);

                        await eventBus.Print(clientId, "设备上报消息", "设备离线");
                    }
                    else if (e.ApplicationMessage.Topic.EndsWith("/connected"))
                    {
                        //处理在线
                        var eventBus = _provider.GetService<ClientBusProxy>();
                        int startIdx = e.ApplicationMessage.Topic.IndexOf("clients/") + 8;
                        int newlen = e.ApplicationMessage.Topic.Length - startIdx - "/connected".Length;
                        string clientId = e.ApplicationMessage.Topic.Substring(startIdx, newlen);
                        string tmppp = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                        OnlineMsg onlinemsg = System.Text.Json.JsonSerializer.Deserialize<OnlineMsg>(tmppp);
                        await eventBus.Connected(clientId, onlinemsg.ipaddress);

                        await eventBus.Print(clientId, "设备上报消息", $"Ip为{onlinemsg.ipaddress}的设备上线");
                    }
                }
                else
                {
                    if (!e.ApplicationMessage.Topic.StartsWith(WUK_STR))
                    {
                        return;
                    }
                    if (e.ApplicationMessage.Topic.Length <= WUK_STR.Length)
                    {
                        return;
                    }

                    string productId = string.Empty;
                    string deviceId = e.ApplicationMessage.Topic.Substring(WUK_STR.Length);
                    string subprefix = string.Empty;
                    int subidx = deviceId.IndexOf("/");
                    if (subidx >= 0)
                    {
                        subprefix = deviceId.Substring(subidx + 1);
                        deviceId = deviceId.Substring(0, subidx);
                    }

                    //限制上报消息与下发消息的间隔
                    var tmpnewdate = DateTime.Now.AddMinutes(-5);
                    _downLastTime.AddOrUpdate(deviceId, tmpnewdate, (k, v) =>
                    {
                        return tmpnewdate;
                    });

                    //解释上报报文
                    var eventBus = _provider.GetService<ClientBusProxy>();
                    if (e.ApplicationMessage.Payload != null)
                    {
                        await eventBus.rawDataTo(productId, deviceId, e.ApplicationMessage.Payload, subprefix);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message + "\n" + ex.StackTrace);
            }

        }

        private ConcurrentDictionary<string, List<RawDataMessage>> _downWaitQueue = new ConcurrentDictionary<string, List<RawDataMessage>>();
        private ConcurrentDictionary<string, DateTime> _downLastTime = new ConcurrentDictionary<string, DateTime>();
        private async Task DownRunHandler(RawDataMessage msg, TslReturn ret)
        {
            var rqlist = _downWaitQueue.GetOrAdd(msg.DeviceId, (key) =>
            {
                return new List<RawDataMessage>();
            });
            if (rqlist.Count > 0)
            {
                var first = rqlist[0];
                rqlist.RemoveAt(0);
                var eventBus = _provider.GetService<ClientBusProxy>();
                await MessageConcurrentHandler(first, ret, true);
            }

        }
        private async Task MessageConcurrentHandler(RawDataMessage msg, TslReturn ret, bool iswait)
        {
            var eventBus = _provider.GetService<ClientBusProxy>();
            if (eventBus == null)
            {
                return;
            }
            bool allowdown = true;
            if (!iswait)
            {
                var lastTime = _downLastTime.GetOrAdd(msg.DeviceId, (key) =>
                {
                    return DateTime.Now.AddHours(-5);
                });
                var rqlist = _downWaitQueue.GetOrAdd(msg.DeviceId, (key) =>
                {
                    return new List<RawDataMessage>();
                });

                int pollTime = eventBus.Option.config.SendInterval;
                var timeSpan = DateTime.Now - lastTime;
                if ((timeSpan.TotalMilliseconds < (pollTime - 20)) || rqlist.Count > 0)
                {
                    allowdown = false;
                    if (!string.IsNullOrEmpty(msg.MessageId))
                    {
                        if (rqlist.Exists(x => x.MessageId == msg.MessageId))
                        {
                            return;
                        }
                    }
                    if (rqlist.Count > 200)
                    {
                        rqlist.Clear();
                    }
                    rqlist.Add(msg);
                    int millsec = rqlist.Count * pollTime;
                    if (millsec > (15 * pollTime))
                    {
                        millsec = 15 * pollTime + (150 * rqlist.Count) % 1000;
                    }
                    _downRuner.PushConcurrentTask(msg, ret, DownRunHandler, TimeSpan.FromMilliseconds(millsec));
                }
            }

            if (allowdown)
            {
                _downLastTime.AddOrUpdate(msg.DeviceId, DateTime.Now.AddHours(-5), (key, old) =>
                {
                    return DateTime.Now;
                });
                if (msg.Data != null)
                {
                    if (msg.Data.Length > 0)
                    {
                        //未发布打印
                        if (ret.Status == "0")
                        {
                            await eventBus.Print(msg.DeviceId, "设备下发消息", FastBufferHelper.ByteToHexStr(msg.Data));
                        }
                        string tdowntopic = $"wukong/down/{msg.DeviceId}";
                        if (!string.IsNullOrEmpty(msg.prefix))
                        {
                            tdowntopic = tdowntopic + "/" + msg.prefix;
                        }
                        var applicationMessage = new MqttApplicationMessageBuilder()
                   .WithTopic(tdowntopic)
                   .WithPayload(msg.Data)
                   .WithRetainFlag(false)
                   .Build();
                        await _client.PublishAsync(applicationMessage);
                    }
                }

            }
        }
        private async Task FirstMessageHandler(RawDataMessage msg, TslReturn ret)
        {
            await MessageConcurrentHandler(msg, ret, false);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var eventBus = _provider.GetService<ClientBusProxy>();
            var option = _provider.GetService<IOptions<MqttOption>>();
            _downRuner = new DownWheelRuner(option.Value.run_count);
            _client = _mqttFactory.CreateMqttClient();
            _client.ApplicationMessageReceivedAsync += MqttServer_ApplicationMessageReceived;

            eventBus.OnSubProductMessage += async (msg, ret) =>
            {
                try
                {
                    if (msg is RawDataMessage rawMsg)
                    {
                        _downRuner.PushConcurrentTask(rawMsg, ret, FirstMessageHandler);
                    }
                }
                catch (Exception ex)
                {
                    await eventBus.Print(msg.DeviceId, "异常", ex.Message);
                }

            };


            var mqttClientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer(option.Value.mqtt_server, option.Value.mqtt_port)
            .WithClientId(Guid.NewGuid().ToString("N"))
            .WithCredentials(option.Value.mqtt_username, option.Value.mqtt_password)
            .Build();

            //断开重连
            _client.DisconnectedAsync += async e =>
            {
                await Task.Delay(TimeSpan.FromSeconds(10));

                try
                {
                    await ConnectMqtt(_client, mqttClientOptions, stoppingToken);
                }
                catch (Exception ex)
                {
                    // Handle reconnect exception
                    Console.WriteLine($"Exception during reconnect: {ex.Message}");
                }
            };

            await ConnectMqtt(_client, mqttClientOptions, stoppingToken);
        }
        private async Task ConnectMqtt(IMqttClient client, MqttClientOptions option, CancellationToken stoppingToken)
        {
            await client.ConnectAsync(option);

            //订阅离在线
            var mqttConnOption = _mqttFactory.CreateSubscribeOptionsBuilder().WithTopicFilter(f =>
            {
                f.WithTopic("$share/ts/$SYS/brokers/+/clients/#");
            }).Build();
            await client.SubscribeAsync(mqttConnOption, stoppingToken);


            //订阅设备消息
            var mqttRecvOption = _mqttFactory.CreateSubscribeOptionsBuilder().WithTopicFilter(f =>
            {
                f.WithTopic("$share/ts/wukong/up/#");
            }).Build();
            await client.SubscribeAsync(mqttRecvOption, stoppingToken);
        }
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            if (_downRuner != null)
            {
                _downRuner.StopAll();
                _downRuner = null;
            }

            var mqttConnOption = _mqttFactory.CreateUnsubscribeOptionsBuilder().WithTopicFilter("$SYS/brokers/+/clients/#").Build();
            _client.UnsubscribeAsync(mqttConnOption, cancellationToken);
            var mqttRecvOption = _mqttFactory.CreateUnsubscribeOptionsBuilder().WithTopicFilter("wukong/up/#").Build();
            _client.UnsubscribeAsync(mqttRecvOption, cancellationToken);
            return base.StopAsync(cancellationToken);
        }
    }

    public class ConfigResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public string Data { get; set; }
    }
    public class OnlineMsg
    {
        public string username { get; set; }
        public long ts { get; set; }
        public int sockport { get; set; }
        public int proto_ver { get; set; }
        public string proto_name { get; set; }
        public int keepalive { get; set; }
        public string ipaddress { get; set; }
        public int expiry_interval { get; set; }
        public long connected_at { get; set; }
        public int connack { get; set; }
        public string clientid { get; set; }
        public bool clean_start { get; set; }
    }
}
