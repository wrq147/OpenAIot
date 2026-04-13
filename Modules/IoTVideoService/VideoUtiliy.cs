using ChannelUtility;
using ChannelUtility.Message;
using Common.EventBus;
using Common.Json;
using IoTService;
using IoTVideoService.Models;
using NATS.Client.Core;

namespace IoTVideoService
{
    public static class VideoUtiliy
    {
        public static async Task DownUpVideoItemMessage(this NatsScope scope, string nodeId, MZ_VideoSource source, string aidata)
        {
            VideoCaptureItem cpitem = new VideoCaptureItem();
            cpitem.Id = source.Id;
            cpitem.PullAddr = source.PullAddr;
            cpitem.PushKey = source.VideoKey;
            cpitem.UserName = source.UserName;
            cpitem.Password = source.UserPwd;
            AIConfig aiConfig;
            if (string.IsNullOrEmpty(aidata))
            {
                aiConfig = new AIConfig();
                aiConfig.MotionRatio = 0.08f;
                aiConfig.CoolDownMs = 200;
                aiConfig.Tasks = new List<AIDetectItem>();
            }
            else
            {
                aiConfig = System.Text.Json.JsonSerializer.Deserialize<AIConfig>(aidata, MyDefaultTextJsonConfig.DefaultOptions);
            }

            if (aiConfig != null)
            {
                aiConfig.OrgId = source.OrgId.Value;
            }

            MediaItemMessage msg = new MediaItemMessage();
            msg.DeviceId = string.Empty;
            msg.ProductId = string.Empty;
            msg.Item = cpitem;
            msg.Config = aiConfig;
            await scope.Public(nodeId, msg);
        }
        public static async Task<List<T_ServerInfo>> GetVideoServers(this IotRedisHelper redis)
        {
            var tmplist = new List<T_ServerInfo>();
            var tmpvideoDict = await redis.HashGetAllAsync<T_ServerInfo>("VideoServers:List");
            if (tmpvideoDict != null)
            {
                foreach (var tmpkvp in tmpvideoDict)
                {
                    if (tmpkvp.Value.Expire >= DateTime.Now)
                    {
                        tmplist.Add(tmpkvp.Value);
                    }
                }
            }
            return tmplist;
        }
        public static async Task<List<T_ServerInfo>> GetGB28181Servers(this IotRedisHelper redis)
        {
            var tmplist = new List<T_ServerInfo>();
            var tmpvideoDict = await redis.HashGetAllAsync<T_ServerInfo>("GB28181Servers:List");
            if (tmpvideoDict != null)
            {
                foreach (var tmpkvp in tmpvideoDict)
                {
                    if (tmpkvp.Value.Expire >= DateTime.Now)
                    {
                        tmplist.Add(tmpkvp.Value);
                    }
                }
            }
            return tmplist;
        }
        public static async Task<List<T_ServerInfo>> GetOnvifServers(this IotRedisHelper redis)
        {
            var tmplist = new List<T_ServerInfo>();
            var tmpvideoDict = await redis.HashGetAllAsync<T_ServerInfo>("OnvifServers:List");
            if (tmpvideoDict != null)
            {
                foreach (var tmpkvp in tmpvideoDict)
                {
                    if (tmpkvp.Value.Expire >= DateTime.Now)
                    {
                        tmplist.Add(tmpkvp.Value);
                    }
                }
            }
            return tmplist;
        }
        public static async Task Public(this NatsScope scope, string nodeId, BaseDeviceMessage msg)
        {
            string msgbody = System.Text.Json.JsonSerializer.Serialize(msg, JsonMessageSerializerConfig.DefaultOptions);
            await scope.Bus.PublishAsync(new NatsMsg<string>()
            {
                Subject = "node." + nodeId,
                Data = msgbody
            }, DefalutNatsJsonSerializer<string>.Default).ConfigureAwait(false);
        }
        public static async Task<T> PublicWait<T>(this NatsScope scope, string nodeId, BaseDeviceMessage msg) where T : class
        {
            INatsSub<T> resSub = null;
            try
            {
                var requestTimeout = TimeSpan.FromSeconds(8);
                resSub = await scope.Bus.SubscribeCoreAsync<T>(msg.MessageId, null, DefalutNatsJsonSerializer<T>.Default, new NatsSubOpts
                {
                    MaxMsgs = 1,
                    Timeout = requestTimeout,
                    ThrowIfNoResponders = true
                }).ConfigureAwait(false);

                string msgbody = System.Text.Json.JsonSerializer.Serialize(msg, JsonMessageSerializerConfig.DefaultOptions);
                await scope.Bus.PublishAsync("node." + nodeId, msgbody, null, msg.MessageId, DefalutNatsJsonSerializer<string>.Default).ConfigureAwait(false);

                await foreach (var responseMsg in resSub.Msgs.ReadAllAsync().ConfigureAwait(false))
                {
                    return responseMsg.Data;
                }
                throw new TimeoutException($"等待 {requestTimeout.TotalSeconds} 秒后未收到回复");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            finally
            {
                if (resSub != null)
                {
                    await resSub.DisposeAsync();
                }
            }
        }
    }
}
