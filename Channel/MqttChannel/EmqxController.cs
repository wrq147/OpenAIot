using ChannelUtility;
using ChannelUtility.Message;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MqttChannel.Timer;
using MQTTnet;
using MQTTnet.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MqttChannel
{
    public class EmqxController : IController
    {
        private IMqttClient _client;
        private IServiceProvider _provider;
        private ILogger<EmqxController> _log;
        private DownWheelRuner _downRuner;
        private MqttFactory _mqttFactory = new MqttFactory();
        private ConcurrentDictionary<string, List<RawDataMessage>> _downWaitQueue = new ConcurrentDictionary<string, List<RawDataMessage>>();
        private ConcurrentDictionary<string, DateTime> _downLastTime = new ConcurrentDictionary<string, DateTime>();
        public EmqxController(IServiceProvider provider)
        {
            _provider = provider;
            _log = _provider.GetService<ILoggerFactory>().CreateLogger<EmqxController>();

        }


        private async Task DownRunHandler(RawDataMessage msg)
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
                await MessageConcurrentHandler(first, true);
            }

        }
        private async Task MessageConcurrentHandler(RawDataMessage msg, bool iswait)
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

                int pollTime = _send_interval;
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
                    _downRuner.PushConcurrentTask(msg, DownRunHandler, TimeSpan.FromMilliseconds(millsec));
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
        private async Task FirstMessageHandler(RawDataMessage msg)
        {
            await MessageConcurrentHandler(msg, false);
        }
        private int _send_interval;


        private async Task DownProductMessage(BaseDeviceMessage msg)
        {
            var eventBus = _provider.GetService<ClientBusProxy>();
            try
            {
                if (msg is RawDataMessage rawMsg)
                {
                    _downRuner.PushConcurrentTask(rawMsg, FirstMessageHandler);
                }
            }
            catch (Exception ex)
            {
                await eventBus.Print(msg.DeviceId, "异常", ex.Message);
            }
        }
        public async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var eventBus = _provider.GetService<ClientBusProxy>();
            var option = _provider.GetService<IOptions<MqttOption>>();
            _send_interval = option.Value.send_interval;
            _downRuner = new DownWheelRuner(option.Value.run_count);
            _client = _mqttFactory.CreateMqttClient();
            _client.ApplicationMessageReceivedAsync += MqttServer_ApplicationMessageReceived;
            eventBus.OnSubProductMessage += DownProductMessage;

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
                    if (!e.ApplicationMessage.Topic.StartsWith(MqttConstants.WUK_STR))
                    {
                        return;
                    }
                    if (e.ApplicationMessage.Topic.Length <= MqttConstants.WUK_STR.Length)
                    {
                        return;
                    }


                    string deviceId = e.ApplicationMessage.Topic.Substring(MqttConstants.WUK_STR.Length);
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

                    //上报报文
                    var eventBus = _provider.GetService<ClientBusProxy>();
                    if (e.ApplicationMessage.Payload != null)
                    {
                        await eventBus.PublishRawUp(deviceId, e.ApplicationMessage.Payload, subprefix);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message + "\n" + ex.StackTrace);
            }

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
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_downRuner != null)
            {
                _downRuner.StopAll();
                _downRuner = null;
            }

            var mqttConnOption = _mqttFactory.CreateUnsubscribeOptionsBuilder().WithTopicFilter("$SYS/brokers/+/clients/#").Build();
            await _client.UnsubscribeAsync(mqttConnOption, cancellationToken);
            var mqttRecvOption = _mqttFactory.CreateUnsubscribeOptionsBuilder().WithTopicFilter("wukong/up/#").Build();
            await _client.UnsubscribeAsync(mqttRecvOption, cancellationToken);
            var eventBus = _provider.GetService<ClientBusProxy>();
            eventBus.OnSubProductMessage -= DownProductMessage;
        }
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
