
using Common;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using MQTTnet.Server;
using System;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace MqttService
{
    /// <summary>
    /// 脚本打印输出
    /// </summary>
    public class MqttController
    {
        private ITAServiceProvider _provider;
        private MqttFactory _mqttFactory = new MqttFactory();
        private MqttServer _server;
        private IMqttClient _client;
        public MqttController(ITAServiceProvider provider, MqttServer server)
        {
            _server = server;
            _provider = provider;
        }

        /// <summary>
        /// 起动服务
        /// </summary>
        /// <returns></returns>
        public void Startup()
        {
            var option = _provider.GetService<IOptions<MqttOption>>();
            _server.ValidatingConnectionAsync += async (x) =>
             {
                 if (x.UserName == option.Value.sys_username && x.Password == option.Value.sys_password)
                 {
                     return;
                 }
                 string rt = await _provider.GetService<GeneralRedisHelper>().StringGetAsync<string>("Client:" + x.ClientId);
                 if (rt == null)
                 {
                     x.ReasonCode = MqttConnectReasonCode.ClientIdentifierNotValid;
                     x.ReasonString = "未知客户端Id";
                     return;
                 }
                 if (x.UserName != rt)
                 {
                     x.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                     x.ReasonString = "未知用户名";
                     return;
                 }
                 x.SessionItems.Add("UserId", rt);
             };
            _server.InterceptingSubscriptionAsync += async (x) =>
            {
                var option = _provider.GetService<IOptions<MqttOption>>();
                string uid = (string)x.SessionItems["UserId"];

                if (uid == option.Value.sys_username)
                {
                    return;
                }
                if (x.TopicFilter.Topic.StartsWith($"user/{uid}") || x.TopicFilter.Topic.StartsWith("console/") || x.TopicFilter.Topic.StartsWith("newprop/") || x.TopicFilter.Topic.StartsWith("newfun/"))
                {
                    return;
                }
                else
                {
                    x.ProcessSubscription = false;
                    x.Response.ReasonCode = MqttSubscribeReasonCode.TopicFilterInvalid;
                    x.Response.ReasonString = "禁止无关订阅";
                    return;
                }
            };
        }
        private async Task<IMqttClient> GetSystemClient()
        {
            if (_client == null)
            {
                var option = _provider.GetService<IOptions<MqttOption>>();
                var client = _mqttFactory.CreateMqttClient();
                var mqttClientOptions = new MqttClientOptionsBuilder()
                  .WithTcpServer("127.0.0.1", option.Value.mqtt_tcp_port)
                  .WithClientId(MyAccess.Core.StringTool.GetGUID())
                  .WithCredentials(option.Value.sys_username, option.Value.sys_password)
                  .Build();
                await client.ConnectAsync(mqttClientOptions);
                _client = client;
            }
            return _client;
        }
        /// <summary>
        /// 通知数据变更
        /// </summary>
        /// <param name="key"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public async Task NoticeData(string key, string msg)
        {
            var client = await GetSystemClient();
            var applicationMessage = new MqttApplicationMessageBuilder()
                .WithTopic(key)
                .WithPayload(msg)
                .Build();

            await client.PublishAsync(applicationMessage);
        }

        /// <summary>
        /// 通知用户有新的消息
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task NoticeUpdate(string uid, string data)
        {
            var client = await GetSystemClient();
            var applicationMessage = new MqttApplicationMessageBuilder()
.WithTopic("user/" + uid + "/new")
.WithPayload(data)
.Build();
            await client.PublishAsync(applicationMessage);

        }
    }
}
