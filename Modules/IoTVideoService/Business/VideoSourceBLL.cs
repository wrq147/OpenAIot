using ChannelUtility.Message;
using Common.EventBus;
using Common.IdGenerator;
using Common.Json;
using Common.Share;
using IoTService;
using IoTService.DAL;
using IoTVideoService.DAL;
using IoTVideoService.Models;
using IoTVideoService.PlanUtil;
using MyAccess.DB.Builder.WhereToSql;
using NATS.Client.Core;
using Quartz;
using System;
using System.Data;
using System.Linq.Expressions;
using System.Text;
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
            Expression<Func<MZ_VideoSource, bool>> expression = x => x.OrgId == user.OrgId && x.VideoType != 2;
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
            data.OrgId = user.OrgId;
            data.NodeId = string.Empty;
            data.UserName ??= string.Empty;
            data.UserPwd ??= string.Empty;

            if (data.VideoType == 0)
            {
                data.Id = "VI_" + snowflake.NextId();
                data.VideoKey = MyAccess.Core.StringTool.GetGUID();
                data.UserName = string.Empty;
                data.UserPwd = string.Empty;
            }
            else if (data.VideoType == 1)
            {
                string tmpid = snowflake.NextId().ToString();
                data.Id = "VI_" + tmpid;
                data.VideoKey = tmpid;
                if (string.IsNullOrEmpty(data.UserName) || string.IsNullOrEmpty(data.UserPwd))
                {
                    return BusResponse<int>.Error(113, "GB28181设备用户名和密码不能为空");
                }
                data.PullAddr = string.Empty;
            }
            else if (data.VideoType == 2)
            {
                data.UserPwd = string.Empty;
                data.PullAddr = string.Empty;
            }

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

            if (!string.IsNullOrEmpty(old.NodeId))
            {
                bool candownVideoItem = false;
                if (data.PullAddr != null && old.PullAddr != data.PullAddr)
                {
                    old.PullAddr = data.PullAddr;
                    candownVideoItem = true;
                }

                if (data.UserName != null && old.UserName != data.UserName)
                {
                    old.UserName = data.UserName;
                    candownVideoItem = true;
                }

                if (data.UserPwd != null && old.UserPwd != data.UserPwd)
                {
                    old.UserPwd = data.UserPwd;
                    candownVideoItem = true;
                }

                if (data.ConfigId != null && old.ConfigId != data.ConfigId)
                {
                    old.ConfigId = data.ConfigId;
                    candownVideoItem = true;
                }

                if (candownVideoItem)
                {
                    await _provider.GetService<NatsScope>().DownUpVideoItemMessage(old.NodeId, old, await _GetAIData(old.ConfigId));
                }
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
                if (info.VideoType == 0)
                {
                    if (!string.IsNullOrEmpty(info.NodeId))
                    {
                        await DownDelVideoItemMessage(info.NodeId, info.Id);
                    }
                }
                var rs = await videoSourceDAL.Delete(x => x.OrgId == user.OrgId && x.Id == id);
                await iotDeviceDAL.Delete(x => x.OrgId == user.OrgId && x.DeviceId == id);

                #region 删除录像计划
                var recordDAL = _provider.GetService<RecordDAL>();
                var rec = (await recordDAL.SelectList(x => x.VideoId == id)).FirstOrDefault();
                if (rec == null)
                {
                    return BusResponse<int>.Success(rs);
                }
                var schedulerFactory = _provider.GetService<ISchedulerFactory>();
                var scheduler = await schedulerFactory.GetScheduler();
                await PlanSchedule.DeleteJob(rec.Id);

                var logDAL = _provider.GetService<RecordLogDAL>();
                await logDAL.Delete(x => x.PlanId == rec.Id);

                var fileDAL = _provider.GetService<RecordFileDAL>();
                await fileDAL.Delete(x => x.PlanId == rec.Id);

                await recordDAL.Delete(rec.Id);
                #endregion

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
            var vvlist = await videoSourceDAL.SelectList(x => x.VideoType == 1 && x.UserName == username);
            if (vvlist.Count > 0)
            {
                return vvlist[0].UserPwd;
            }
            else
            {
                return null;
            }
        }

   
        private async Task DownDelVideoItemMessage(string nodeId, string videoId)
        {
            MediaDelItemMessage msg = new MediaDelItemMessage();
            msg.DeviceId = videoId;
            msg.ProductId = string.Empty;
            await _provider.GetService<NatsScope>().Public(nodeId, msg);
        }
        public async Task ResponseVerifyResult(string msgId, string rs)
        {
            var bus = _provider.GetService<NatsScope>().Bus;
            await bus.PublishAsync(new NatsMsg<string>()
            {
                Subject = msgId,
                Data = rs
            }, DefalutNatsJsonSerializer<string>.Default).ConfigureAwait(false);
        }
        public virtual async Task DelVideo(MediaNotReaderMessage msg)
        {
            var videoSourceDAL = _provider.GetService<VideoSourceDAL>();
            string tkey = msg.StreamId;
            if (msg.VideoType == 0)
            {
                var tlist = await videoSourceDAL.SelectList(x => x.VideoType == 0 && x.VideoKey == tkey);
                if (tlist.Count > 0)
                {
                    if (!string.IsNullOrEmpty(tlist[0].NodeId))
                    {
                        MZ_VideoSource tsource = new MZ_VideoSource();
                        tsource.NodeId = string.Empty;
                        tsource.Id = tlist[0].Id;
                        await videoSourceDAL.Update(tsource);
                        await DownDelVideoItemMessage(tlist[0].NodeId, tlist[0].Id);
                    }
                }
            }
            else if (msg.VideoType == 1)
            {
                MZ_VideoSource videoSource = new MZ_VideoSource();
                videoSource.NodeId = string.Empty;
                await videoSourceDAL.Update(videoSource, x => x.VideoType == 1 && x.UserName == msg.StreamId);
                await videoSourceDAL.Delete(x => x.VideoType == 2 && x.UserName == msg.StreamId);
            }
        }
        public virtual async Task InitFixNode(string nodeId)
        {
            var videoSourceDAL = _provider.GetService<VideoSourceDAL>();
            var sourceList = await videoSourceDAL.SelectList(x => x.NodeId == nodeId && x.VideoType == 0);
            foreach (var source in sourceList)
            {
                //判断通道是否在录制中，如果是则重新启用录制
                var recInfo = (await _provider.GetService<RecordDAL>().SelectList(x => x.VideoId == source.Id)).FirstOrDefault();
                if (recInfo != null)
                {
                    var tasks = PlanTimeParser.GenerateTriggerTasks(recInfo);
                    if (tasks.Count > 0)
                    {
                        if (PlanTimeParser.GetTodayNextTriggerTask(tasks) != null)
                        {
                            //直接开始录像
                            await _provider.GetService<RecordBLL>().PublishStartRecordMessage(recInfo.Id, recInfo.StorageWay.Value, DateTime.Now, source);
                        }
                    }
                }
            }
        }
        public virtual async Task InitChannels(string userName, int videoType, string nodeId, List<ChannelData> channels)
        {
            var videoSourceDAL = _provider.GetService<VideoSourceDAL>();
            await videoSourceDAL.Delete(x => x.VideoType == 2 && x.UserName == userName);
            var parentSource = (await videoSourceDAL.SelectList(x => x.VideoType == videoType && x.UserName == userName)).FirstOrDefault();
            if (parentSource == null)
            {
                return;
            }
            List<MZ_VideoSource> channelSources = new List<MZ_VideoSource>();
            for (int i = 0; i < channels.Count; i++)
            {
                var channel = channels[i];
                MZ_VideoSource videoSource = new MZ_VideoSource();
                videoSource.Id = parentSource.Id + "_" + channel.Index;
                videoSource.OrgId = parentSource.OrgId;
                videoSource.VideoType = 2;
                videoSource.VideoKey = parentSource.VideoKey + "_" + channel.ChannelId;
                videoSource.Position = channel.Name;
                videoSource.PullAddr = string.Empty;
                videoSource.UserName = userName;
                videoSource.UserPwd = string.Empty;
                videoSource.NodeId = nodeId;
                videoSource.ConfigId = string.Empty;
                channelSources.Add(videoSource);
            }

            if (channelSources.Count > 0)
            {
                await videoSourceDAL.Insert(channelSources);
            }

            //判断通道是否在录制中，如果是则重新启用录制
            var recInfo = (await _provider.GetService<RecordDAL>().SelectList(x => x.VideoId == parentSource.Id)).FirstOrDefault();
            if (recInfo != null)
            {
                var tasks = PlanTimeParser.GenerateTriggerTasks(recInfo);
                if (tasks.Count > 0)
                {
                    if (PlanTimeParser.GetTodayNextTriggerTask(tasks) != null)
                    {
                        //直接开始录像
                        await _provider.GetService<RecordBLL>().PublishStartRecordMessage(recInfo.Id, recInfo.StorageWay.Value, DateTime.Now, parentSource);
                    }
                }
            }
        }

        private async Task<string> _GetAIData(string configId)
        {
            string aidata = string.Empty;
            if (!string.IsNullOrEmpty(configId))
            {
                var configData = await _provider.GetService<VideoConfigDAL>().Select(configId);
                if (configData != null)
                {
                    aidata = configData.AITasks;
                }
            }
            return aidata;
        }
        public virtual async Task CollectVideo(MediaNotFoundMessage msg)
        {
            var videoSourceDAL = _provider.GetService<VideoSourceDAL>();
            //给在线节点分配视频采集
            string tkey = msg.StreamId;
            List<MZ_VideoSource> tlist;
            if (msg.VideoType == 0)
            {
                tlist = await videoSourceDAL.SelectList(x => x.VideoType == 0 && x.VideoKey == tkey);
                if (tlist.Count > 0)
                {
                    var titem = tlist[0];
                    MZ_VideoSource tsource = new MZ_VideoSource();
                    tsource.NodeId = msg.DeviceId;
                    tsource.Id = titem.Id;
                    await videoSourceDAL.Update(tsource);

                    await _provider.GetService<NatsScope>().DownUpVideoItemMessage(msg.DeviceId, titem, await _GetAIData(titem.ConfigId));
                }
            }
            else if (msg.VideoType == 1)
            {
                tlist = await videoSourceDAL.SelectList(x => x.VideoType == 1 && x.UserName == tkey);
                if (tlist.Count > 0)
                {
                    MZ_VideoSource tsource = new MZ_VideoSource();
                    tsource.NodeId = msg.DeviceId;
                    tsource.Id = tlist[0].Id;
                    await videoSourceDAL.Update(tsource);

                    await _provider.GetService<NatsScope>().DownUpVideoItemMessage(msg.DeviceId, tlist[0], await _GetAIData(tlist[0].ConfigId));
                }
            }
            else if (msg.VideoType == 3)
            {
                tlist = await videoSourceDAL.SelectList(x => x.VideoType == 3 && x.VideoKey == tkey);
                if (tlist.Count > 0)
                {
                    var titem = tlist[0];
                    MZ_VideoSource tsource = new MZ_VideoSource();
                    tsource.NodeId = msg.DeviceId;
                    tsource.Id = titem.Id;
                    await videoSourceDAL.Update(tsource);

                    await _provider.GetService<NatsScope>().DownUpVideoItemMessage(msg.DeviceId, titem, await _GetAIData(titem.ConfigId));
                }
            }
            else
            {
                return;
            }


        }

        public virtual async Task TimerClean()
        {
            //清除过期录像文件
            var minDate = DateTime.Now.Date;
            var recordLogDAL = _provider.GetService<RecordLogDAL>();
            var recordDAL = _provider.GetService<RecordDAL>();
            var recordFileDAL = _provider.GetService<RecordFileDAL>();
            var snowflake = _provider.GetService<SnowflakeHelper>();
            var recordBLL = _provider.GetService<RecordBLL>();
            var sourceDAL = _provider.GetService<VideoSourceDAL>();
            var records = await recordDAL.GetUnCleanRecords(minDate, 2000);
            while (records.Count > 0)
            {
                string startId = snowflake.NextId().ToString();
                int i = 0;
                foreach (var rec in records)
                {
                    MZ_IotRecordLog log = new MZ_IotRecordLog();
                    log.Id = startId + "_" + i;
                    log.PlanId = rec.Id;
                    log.Position = rec.Position;
                    log.VideoId = rec.VideoId;
                    log.LogType = "clean";
                    log.Content = string.Empty;
                    log.ExecTime = DateTime.Now;
                    await recordLogDAL.Insert(log);

                    DateTime overTime = DateTime.Now.Date.AddDays(-rec.SaveCycle.Value);
                    var recFiles = await recordFileDAL.SelectList(x => x.PlanId == rec.Id && x.FileDate < overTime);
                    var recGroups = recFiles.GroupBy(x => x.NodeId);
                    foreach (var recItem in recGroups)
                    {
                        await recordBLL.PublishCleanRecordMessage(recItem.Key, rec.VideoId, recItem.ToList());
                    }
                    ++i;
                }
                records = await recordDAL.GetUnCleanRecords(minDate, 1000);
            }


            //清除过期视频服务节点
            IotRedisHelper redis = _provider.GetService<IotRedisHelper>();
            var tmpvideoDict = await redis.HashGetAllAsync<T_ServerInfo>("VideoServers:List");
            if (tmpvideoDict != null)
            {
                foreach (var tmpkvp in tmpvideoDict)
                {
                    if (tmpkvp.Value.Expire < DateTime.Now)
                    {
                        tmpvideoDict.Remove(tmpkvp.Key);
                    }
                }
                await redis.HashSetAsync("VideoServers:List", tmpvideoDict);
            }

            var tmpgb28181Dict = await redis.HashGetAllAsync<T_ServerInfo>("GB28181Servers:List");
            if (tmpgb28181Dict != null)
            {
                foreach (var tmpkvp in tmpgb28181Dict)
                {
                    if (tmpkvp.Value.Expire < DateTime.Now)
                    {
                        tmpgb28181Dict.Remove(tmpkvp.Key);
                    }
                }
                await redis.HashSetAsync("GB28181Servers:List", tmpgb28181Dict);
            }

            var tmponvifDict = await redis.HashGetAllAsync<T_ServerInfo>("OnvifServers:List");
            if (tmponvifDict != null)
            {
                foreach (var tmpkvp in tmponvifDict)
                {
                    if (tmpkvp.Value.Expire < DateTime.Now)
                    {
                        tmponvifDict.Remove(tmpkvp.Key);
                    }
                }
                await redis.HashSetAsync("OnvifServers:List", tmponvifDict);
            }
        }
    }
}
