using ChannelUtility;
using ChannelUtility.Message;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MqttChannel.Timer;
using MQTTnet;
using MQTTnet.Server;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MqttChannel
{
    public class MqttController : IController
    {
        private IServiceProvider _provider;
        private MqttServer _server;
        private ILogger<MqttController> _log;
        private DownWheelRuner _downRuner;
        private ConcurrentDictionary<string, List<RawDataMessage>> _downWaitQueue = new ConcurrentDictionary<string, List<RawDataMessage>>();
        private ConcurrentDictionary<string, DateTime> _downLastTime = new ConcurrentDictionary<string, DateTime>();
        public MqttController(IServiceProvider provider)
        {
            _provider = provider;
            _log = _provider.GetService<ILoggerFactory>().CreateLogger<MqttController>();
        }
        private bool _isStart = false;
        /// <summary>
        /// 起动服务
        /// </summary>
        /// <returns></returns>
        public async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var option = _provider.GetService<IOptions<MqttOption>>();
            var options = new MqttServerOptionsBuilder()
            .WithDefaultEndpoint()
            .WithDefaultEndpointPort(option.Value.mqtt_port)
            .Build();
            _server = new MqttFactory().CreateMqttServer(options);

            _server.ValidatingConnectionAsync += async (x) =>
            {
                if (x.UserName == option.Value.mqtt_username && x.Password == option.Value.mqtt_password)
                {
                    return;
                }
                x.SessionItems.Add("UserId", x.ClientId);
            };
            _server.ClientConnectedAsync += MqttServer_ClientConnected;
            _server.ClientDisconnectedAsync += MqttServer_ClientDisconnected;
            _server.InterceptingPublishAsync += MqttServer_ApplicationMessageReceived;
            await _server.StartAsync();
            var eventBus = _provider.GetService<ClientBusProxy>();
            eventBus.OnSubProductMessage += DownProductMessage;
            _isStart = true;
        }
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
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_downRuner != null)
            {
                _downRuner.StopAll();
                _downRuner = null;
            }
            if (_isStart)
            {
                await _server.StopAsync();
                var eventBus = _provider.GetService<ClientBusProxy>();
                eventBus.OnSubProductMessage -= DownProductMessage;
                _isStart = false;
            }
        }
        private async Task MqttServer_ClientConnected(ClientConnectedEventArgs e)
        {
            //处理在线
            var eventBus = _provider.GetService<ClientBusProxy>();
            string clientId = e.ClientId;
            int ipeidx = e.Endpoint.IndexOf(':');
            string ipaddr = string.Empty;
            if (ipeidx > 0)
            {
                ipaddr = e.Endpoint.Substring(0, ipeidx);
            }

            await eventBus.Connected(clientId, ipaddr);
            await eventBus.Print(clientId, "设备上报消息", $"Ip为{ipaddr}的设备上线");
        }
        private async Task MqttServer_ClientDisconnected(ClientDisconnectedEventArgs e)
        {
            //处理离线
            var eventBus = _provider.GetService<ClientBusProxy>();
            string clientId = e.ClientId;
            await eventBus.Disconnect(clientId);
            _downWaitQueue.TryRemove(clientId, out List<RawDataMessage> requests);
            _downLastTime.TryRemove(clientId, out DateTime tmpdt);

            await eventBus.Print(clientId, "设备上报消息", "设备离线");
        }
        private async Task MqttServer_ApplicationMessageReceived(InterceptingPublishEventArgs e)
        {
            string topic = e.ApplicationMessage.Topic;
            if (!topic.StartsWith(MqttConstants.WUK_STR))
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

            //解释上报报文
            var eventBus = _provider.GetService<ClientBusProxy>();
            if (e.ApplicationMessage.Payload != null)
            {
                await eventBus.PublishRawUp(deviceId, e.ApplicationMessage.Payload, subprefix);
            }
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
                        await PublishMessage(tdowntopic, msg.Data);
                    }
                }

            }
        }
        private async Task FirstMessageHandler(RawDataMessage msg)
        {
            await MessageConcurrentHandler(msg, false);
        }
        private int _send_interval;
        public async Task PublishMessage(string topic, byte[] payload)
        {
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(payload)
                .Build();

            // 通过服务器的内部通道发布
            await _server.InjectApplicationMessage(new InjectedMqttApplicationMessage(message)
            {
                SenderClientId = "MqttServer" // 标记消息来自服务器
            });
        }
    }
}
