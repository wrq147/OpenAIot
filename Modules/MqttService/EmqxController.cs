using Common;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TemplateAction.Common;
using TemplateAction.Core;

namespace MqttService
{
    public class EmqxController
    {
        private ITAServiceProvider _provider;
        private MqttFactory _mqttFactory = new MqttFactory();
        private IMqttClient _client;
        public EmqxController(ITAServiceProvider provider)
        {
            _provider = provider;
            var option = _provider.GetService<IOptions<MqttOption>>();
            var client = _mqttFactory.CreateMqttClient();
            var mqttClientOptions = new MqttClientOptionsBuilder()
              .WithTcpServer(option.Value.mqtt_tcp_server, option.Value.mqtt_tcp_port)
              .WithClientId(MyAccess.Core.StringTool.GetGUID())
              .WithCredentials(option.Value.sys_username, option.Value.sys_password)
              .Build();

            TAAsyncHelper.RunSync(async () =>
            {
                await client.ConnectAsync(mqttClientOptions).ConfigureAwait(false);

                var mqttSubscribeOptions = _mqttFactory.CreateSubscribeOptionsBuilder()
  .WithTopicFilter(
      f =>
      {
          f.WithTopic("$SYS/brokers/+/clients/#");
      })
  .Build();
                var response = await client.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None).ConfigureAwait(false);
            });
            _client = client;


        }



        public async Task NoticeData(string key, string msg)
        {
            var applicationMessage = new MqttApplicationMessageBuilder()
                .WithTopic(key)
                .WithPayload(msg)
                .Build();

            await _client.PublishAsync(applicationMessage);
        }

        public async Task NoticeUpdate(string uid)
        {
            var applicationMessage = new MqttApplicationMessageBuilder()
    .WithTopic("user/" + uid + "/new")
    .WithPayload(string.Empty)
    .Build();
            await _client.PublishAsync(applicationMessage);
        }
    }
}
