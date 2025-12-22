using ChannelUtility;
using ChannelUtility.Message;
using Common.EventBus;
using EasyNetQ;
using IoTService;
using IoTVideoService.DAL;
using IoTVideoService.Models;
using Microsoft.Extensions.Options;
using MyAccess.DB.Builder.WhereToSql;
using Quartz.Impl.Triggers;
using System;
using System.Text;
using TemplateAction.Core;

namespace IoTVideoService.Business
{
    public class FixVideoBLL
    {
        private ITAServiceProvider _provider;
        public FixVideoBLL(ITAServiceProvider provider)
        {
            _provider = provider;
        }
        private async Task DownUpVideoItemMessage(string nodeid, VideoCaptureItem item, List<AIDetectItem> detectList)
        {
            UpVideoItemMessage msg = new UpVideoItemMessage();
            msg.DeviceId = string.Empty;
            msg.ProductId = string.Empty;
            msg.Item = item;
            msg.DetectList = detectList;
            var bus = _provider.GetService<RabbitScope>().Bus;
            string msgbody = System.Text.Json.JsonSerializer.Serialize(msg, JsonMessageSerializerConfig.DefaultOptions);
            await bus.PubSub.PublishAsync(msgbody, "/device." + nodeid + ".guid").ConfigureAwait(false);
        }
        private async Task DownDelVideoItemMessage(string nodeid, string videoId)
        {
            DelVideoItemMessage msg = new DelVideoItemMessage();
            msg.DeviceId = videoId;
            msg.ProductId = string.Empty;
            var bus = _provider.GetService<RabbitScope>().Bus;
            string msgbody = System.Text.Json.JsonSerializer.Serialize(msg, JsonMessageSerializerConfig.DefaultOptions);
            await bus.PubSub.PublishAsync(msgbody, "/device." + nodeid + ".guid").ConfigureAwait(false);
        }
        private string GeneratePushAddr(string key)
        {
            var tmpoption = _provider.GetService<IOptions<VideoOption>>();
            var tservers = tmpoption.Value.Servers;
            int pos = Math.Abs(key.GetHashCode() % tservers.Length);

            return $"{tservers[pos].ZLMediaKitIp}:{tservers[pos].ZLMediaKitRTMPPort}/{tservers[pos].ZLMediaKitApp}/{key}";
        }
        public virtual async Task DelVideo(string streamId)
        {
            var videoSourceDAL = _provider.GetService<VideoSourceDAL>();
            string tkey = streamId;
            var tlist = await videoSourceDAL.SelectList(x => x.VideoType == 0 && x.VideoKey == tkey);
            if (tlist.Count > 0)
            {
                if (!string.IsNullOrEmpty(tlist[0].PullNode))
                {
                    MZ_VideoSource tsource = new MZ_VideoSource();
                    tsource.PullNode = string.Empty;
                    tsource.Id = tlist[0].Id;
                    await videoSourceDAL.Update(tsource);
                    await DownDelVideoItemMessage(tlist[0].PullNode, tlist[0].Id);
                }
            }
        }
        public virtual async Task CollectVideo(string streamId)
        {
            var videoSourceDAL = _provider.GetService<VideoSourceDAL>();
            //获取所有固定地址采集节点
            var redisHelper = _provider.GetService<IotRedisHelper>();
            var dict = await redisHelper.HashGetAllAsync<string>("FixVideoNode");
            var serverBus = _provider.GetService<ServerBusProxy>();
            List<string> offlineNames = new List<string>();
            List<string> onlineNames = new List<string>();
            foreach (var kvp in dict)
            {
                if (DateTime.TryParse(kvp.Value, out DateTime dt))
                {
                    if (dt < DateTime.Now)
                    {
                        offlineNames.Add(kvp.Key);
                    }
                    else
                    {
                        onlineNames.Add(kvp.Key);
                    }
                }
                else
                {
                    offlineNames.Add(kvp.Key);
                }
            }
            if (offlineNames.Count > 0)
            {
                await redisHelper.HashDeleteAsync("FixVideoNode", offlineNames.ToArray());
                MZ_VideoSource vs = new MZ_VideoSource();
                vs.PullNode = string.Empty;
                await videoSourceDAL.Update(vs, x => onlineNames.NotContains(x.PullNode));
            }
            if (onlineNames == null || onlineNames.Count == 0) return;

            //给在线节点分配视频采集
            string tkey = streamId;
            var tlist = await videoSourceDAL.SelectList(x => x.VideoType == 0 && x.VideoKey == tkey);
            if (tlist.Count > 0)
            {
                var titem = tlist[0];
                int pos = Math.Abs(titem.Id.GetHashCode() % onlineNames.Count);
                string nodeid = onlineNames[pos];
                MZ_VideoSource tsource = new MZ_VideoSource();
                tsource.PullNode = nodeid;
                tsource.Id = titem.Id;
                await videoSourceDAL.Update(tsource);
                VideoCaptureItem cpitem = new VideoCaptureItem();
                cpitem.Id = titem.Id;
                cpitem.FrameInterval = titem.FrameInterval.Value;
                cpitem.PullAddr = titem.PullAddr;
                cpitem.PushAddr = GeneratePushAddr(titem.VideoKey);
                List<AIDetectItem> detectList;
                if (string.IsNullOrEmpty(titem.AITasks))
                {
                    detectList = new List<AIDetectItem>();
                }
                else
                {
                    detectList = System.Text.Json.JsonSerializer.Deserialize<List<AIDetectItem>>(titem.AITasks);
                }

                await DownUpVideoItemMessage(nodeid, cpitem, detectList);
            }

        }


    }
}
