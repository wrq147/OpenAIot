using ChannelUtility;
using ChannelUtility.Message;
using Common.EventBus;
using NATS.Client.Core;

namespace IoTVideoService
{
    public static class VideoUtiliy
    {
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
