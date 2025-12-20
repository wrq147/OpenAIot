using ChannelUtility;
using ChannelUtility.Message;
using Common.EventBus;
using EasyNetQ;
using IoTService;
using IoTVideoService.DAL;
using IoTVideoService.Models;
using Microsoft.Extensions.Options;
using MyAccess.DB.Builder.WhereToSql;
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
        private string GeneratePushAddr(string pullAddr, string key)
        {
            var tmpoption = _provider.GetService<IOptions<VideoOption>>();
            var trimAddr = pullAddr.Trim();
            var uri = new Uri(trimAddr);
            var tsche = uri.Scheme.ToLowerInvariant();
            if (tsche == "rtmp")
            {
                return $"rtmp://{tmpoption.Value.ZLMediaKitIp}:{tmpoption.Value.ZLMediaKitRTMPPort}/{tmpoption.Value.ZLMediaKitApp}/{key}";
            }
            else if (tsche == "rtsp")
            {
                return $"rtsp://{tmpoption.Value.ZLMediaKitIp}:{tmpoption.Value.ZLMediaKitRTMPPort}/{tmpoption.Value.ZLMediaKitApp}/{key}";
            }
            else
            {
                return string.Empty;
            }
        }
        public virtual async Task CollectVideo()
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
            In_VideoSourcePage query = new In_VideoSourcePage();
            query.pageNum = 1;
            query.pageSize = 1000;
            var tpagelist = await videoSourceDAL.SelectPage(x => x.VideoType == 0 && x.PullNode == "", query, string.Empty);
            foreach (var titem in tpagelist.List)
            {
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
                cpitem.PushAddr = GeneratePushAddr(titem.PullAddr, titem.VideoKey);
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
            if (tpagelist.List.Count > 0)
            {
                await this.CollectVideo();
            }
        }
    }
}
