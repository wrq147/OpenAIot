using ChannelUtility;
using ChannelUtility.Message;
using Common.EventBus;
using Common.IdGenerator;
using Common.Json;
using Common.Share;
using EasyNetQ;
using IoTService;
using IoTService.DAL;
using IoTVideoService.DAL;
using IoTVideoService.Models;
using Microsoft.Extensions.Options;
using MyAccess.DB.Builder.WhereToSql;
using System;
using System.Linq.Expressions;
using TemplateAction.Core;

namespace IoTVideoService.Business
{
    public class VideoSourceBLL
    {
        private ITAServiceProvider _provider;
        public VideoSourceBLL(ITAServiceProvider provider)
        {
            _provider = provider;
        }

        public virtual async Task<PageObject<MZ_VideoSource>> SelectPage(In_VideoSourcePage query, IUserInfo user)
        {
            Expression<Func<MZ_VideoSource, bool>> expression = x => x.OrgId == user.OrgId;
            if (!string.IsNullOrEmpty(query.Key))
            {
                expression = expression.And(x => x.Position.Contains(query.Key));
            }
            var rsp = await _provider.GetService<VideoSourceDAL>().SelectPage(expression, query, string.Empty);
            return rsp;
        }
        public virtual async Task<BusResponse<int>> Insert(MZ_VideoSource data, IUserInfo user)
        {
            if (user.OrgId <= 0)
            {
                return BusResponse<int>.Error(112, "非企业用户无法添加视频源");
            }
            var snowflake = _provider.GetService<SnowflakeHelper>();
            data.Id = "VI-" + snowflake.NextId();
            data.OrgId = user.OrgId;
            data.VideoKey = MyAccess.Core.StringTool.GetGUID();
            data.PullNode = string.Empty;
            data.NodeId = string.Empty;
            data.AITasks ??= string.Empty;
            data.GBPublicAddr ??= string.Empty;
            data.GBPublicPort ??= 0;

            int rs = await _provider.GetService<VideoSourceDAL>().Insert(data);
            return BusResponse<int>.Success(rs);
        }


        public virtual async Task<BusResponse<MZ_VideoSource>> Info(string id)
        {
            var videoSourceDAL = _provider.GetService<VideoSourceDAL>();
            var info = await videoSourceDAL.Select(id);
            if (info == null)
            {
                return BusResponse<MZ_VideoSource>.Error(111, "视频源不存在");
            }
            ServerInfo serverInfo = null;
            if (string.IsNullOrEmpty(info.NodeId))
            {
                if (info.VideoType == 0)
                {
                    var option = _provider.GetService<IOptions<VideoOption>>();
                    if (option.Value.VideoServers.Count > 0)
                    {
                        int pos = Math.Abs(id.GetHashCode() % option.Value.VideoServers.Count);
                        serverInfo = option.Value.VideoServers[pos];
                    }
                }
            }
            else
            {
                var option = _provider.GetService<IOptions<VideoOption>>();
                serverInfo = option.Value.VideoServers.Where(x => x.NodeId == info.NodeId).FirstOrDefault();
            }
            if (serverInfo != null)
            {
                info.VideoUrl = $"rtmp://{serverInfo.Ip}:{serverInfo.Port}/live/{info.VideoKey}";
            }
            else
            {
                info.VideoUrl = string.Empty;
            }
            return BusResponse<MZ_VideoSource>.Success(info);
        }
        public virtual async Task<BusResponse<int>> Update(MZ_VideoSource data, IUserInfo user)
        {
            if (user.OrgId <= 0)
            {
                return BusResponse<int>.Error(112, "非企业用户无法修改视频源");
            }
            var videoSourceDAL = _provider.GetService<VideoSourceDAL>();
            var old = await videoSourceDAL.Select(data.Id);
            if (old == null)
            {
                return BusResponse<int>.Error(113, "视频源不存在");
            }
            if (old.OrgId != user.OrgId)
            {
                return BusResponse<int>.Error(114, "无权修改当前视频源");
            }
            data.VideoKey = null;
            data.OrgId = null;
            data.PullNode = null;

            if (old.VideoType == 0 && !string.IsNullOrEmpty(data.PullAddr) && old.PullAddr != data.PullAddr && !string.IsNullOrEmpty(old.PullNode))
            {
                //更新拉流
                old.PullAddr = data.PullAddr;
                await DownUpVideoItemMessage(old.PullNode, old);
            }
            return BusResponse<int>.Success(await videoSourceDAL.Update(data));
        }

        public virtual async Task<BusResponse<int>> Delete(string id, IUserInfo user)
        {
            if (user.OrgId <= 0)
            {
                return BusResponse<int>.Error(112, "非企业用户无法删除视频源");
            }
            try
            {
                var videoSourceDAL = _provider.GetService<VideoSourceDAL>();
                var iotDeviceDAL = _provider.GetService<IotDeviceDAL>();
                var info = await videoSourceDAL.Select(id);
                if (info == null)
                {
                    return BusResponse<int>.Error(111, "视频源不存在");
                }
                if (!string.IsNullOrEmpty(info.PullNode))
                {
                    await DownDelVideoItemMessage(info.PullNode, info.Id);
                }
                var rs = await videoSourceDAL.Delete(x => x.OrgId == user.OrgId && x.Id == id);
                await iotDeviceDAL.Delete(x => x.OrgId == user.OrgId && x.DeviceId == id);

                return BusResponse<int>.Success(rs);
            }
            catch (Exception ex)
            {
                return BusResponse<int>.Error(111, ex.Message);
            }
        }
        public virtual async Task<string> GB28181Login(string username)
        {
            var videoSourceDAL = _provider.GetService<VideoSourceDAL>();
            var vvlist = await videoSourceDAL.SelectList(x => x.UserName == username);
            if (vvlist.Count > 0)
            {
                return vvlist[0].UserPwd;
            }
            else
            {
                return null;
            }
        }

        private async Task DownUpVideoItemMessage(string nodeguid, MZ_VideoSource source)
        {
            VideoCaptureItem cpitem = new VideoCaptureItem();
            cpitem.Id = source.Id;
            cpitem.PullAddr = source.PullAddr;
            cpitem.PushKey = source.VideoKey;
            cpitem.UserName = source.UserName;
            AIConfig aiConfig;
            if (string.IsNullOrEmpty(source.AITasks))
            {
                aiConfig = new AIConfig();
                aiConfig.MotionRatio = 0.08f;
                aiConfig.CoolDownMs = 200;
                aiConfig.Tasks = new List<AIDetectItem>();
            }
            else
            {
                aiConfig = System.Text.Json.JsonSerializer.Deserialize<AIConfig>(source.AITasks, MyDefaultTextJsonConfig.DefaultOptions);
            }

            MediaItemMessage msg = new MediaItemMessage();
            msg.DeviceId = string.Empty;
            msg.ProductId = string.Empty;
            msg.Item = cpitem;
            msg.Config = aiConfig;
            var bus = _provider.GetService<RabbitScope>().Bus;
            string msgbody = System.Text.Json.JsonSerializer.Serialize(msg, JsonMessageSerializerConfig.DefaultOptions);
            await bus.PubSub.PublishAsync(msgbody, "/node." + nodeguid).ConfigureAwait(false);
        }
        private async Task DownDelVideoItemMessage(string nodeguid, string videoId)
        {
            MediaDelItemMessage msg = new MediaDelItemMessage();
            msg.DeviceId = videoId;
            msg.ProductId = string.Empty;
            var bus = _provider.GetService<RabbitScope>().Bus;
            string msgbody = System.Text.Json.JsonSerializer.Serialize(msg, JsonMessageSerializerConfig.DefaultOptions);
            await bus.PubSub.PublishAsync(msgbody, "/node." + nodeguid).ConfigureAwait(false);
        }
        public async Task ResponseVerifyResult(string msgId, string rs)
        {
            var bus = _provider.GetService<RabbitScope>().Bus;
            await bus.SendReceive.SendAsync("bus.response." + msgId, rs).ConfigureAwait(false);
        }
        public virtual async Task DelVideo(MediaNotReaderMessage msg)
        {
            var videoSourceDAL = _provider.GetService<VideoSourceDAL>();
            string tkey = msg.StreamId;
            var tlist = await videoSourceDAL.SelectList(x => x.VideoType == 0 && x.VideoKey == tkey);
            if (tlist.Count > 0)
            {
                if (!string.IsNullOrEmpty(tlist[0].PullNode))
                {
                    MZ_VideoSource tsource = new MZ_VideoSource();
                    tsource.PullNode = string.Empty;
                    tsource.NodeId = string.Empty;
                    tsource.Id = tlist[0].Id;
                    await videoSourceDAL.Update(tsource);
                    await DownDelVideoItemMessage(tlist[0].PullNode, tlist[0].Id);
                }
            }
        }
        public async Task UpdateFixNode()
        {
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
                var videoSourceDAL = _provider.GetService<VideoSourceDAL>();
                await videoSourceDAL.Update(vs, x => x.VideoType == 0 && onlineNames.NotContains(x.PullNode));
            }
        }

        public virtual async Task CollectVideo(MediaNotFoundMessage msg)
        {
            await this.UpdateFixNode();

            var videoSourceDAL = _provider.GetService<VideoSourceDAL>();
            //给在线节点分配视频采集
            string tkey = msg.StreamId;
            List<MZ_VideoSource> tlist;
            if (msg.VideoType == 0)
            {
                tlist = await videoSourceDAL.SelectList(x => x.VideoType == 0 && x.VideoKey == tkey);
            }
            else if (msg.VideoType == 1)
            {
                tlist = await videoSourceDAL.SelectList(x => x.VideoType == 1 && x.UserName == tkey);
            }
            else
            {
                return;
            }
            if (tlist.Count > 0)
            {
                var titem = tlist[0];
                string nodeguid = msg.NodeGuid;
                if (titem.VideoType == 0 && !string.IsNullOrEmpty(titem.PullNode) && !string.Equals(titem.PullNode, nodeguid))
                {
                    await DownDelVideoItemMessage(titem.PullNode, titem.Id);
                }
                MZ_VideoSource tsource = new MZ_VideoSource();
                tsource.PullNode = nodeguid;
                tsource.NodeId = msg.DeviceId;
                tsource.Id = titem.Id;
                await videoSourceDAL.Update(tsource);

                await DownUpVideoItemMessage(nodeguid, titem);
            }

        }


    }
}
