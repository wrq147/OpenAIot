using ChannelUtility;
using ChannelUtility.Message;
using Common.EventBus;
using Common.IdGenerator;
using Common.Json;
using Common.Share;
using InfluxDB.Client.Api.Domain;
using IoTService;
using IoTService.DAL;
using IoTVideoService.DAL;
using IoTVideoService.Models;
using IoTVideoService.PlanUtil;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Writers;
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
            data.AITasks ??= string.Empty;
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

            if (old.VideoType == 0 && !string.IsNullOrEmpty(data.PullAddr) && old.PullAddr != data.PullAddr && !string.IsNullOrEmpty(old.NodeId))
            {
                //更新拉流
                old.PullAddr = data.PullAddr;
                await DownUpVideoItemMessage(old.NodeId, old);
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

        private async Task DownUpVideoItemMessage(string nodeId, MZ_VideoSource source)
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
            await _provider.GetService<NatsScope>().Public(nodeId, msg);
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
        public virtual async Task InitChannels(string userName, string nodeId, List<ChannelData> channels)
        {
            var videoSourceDAL = _provider.GetService<VideoSourceDAL>();
            await videoSourceDAL.Delete(x => x.VideoType == 2 && x.UserName == userName);
            var parentSource = (await videoSourceDAL.SelectList(x => x.VideoType == 1 && x.UserName == userName)).FirstOrDefault();
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
                videoSource.AITasks = string.Empty;
                videoSource.NodeId = nodeId;
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

                    await DownUpVideoItemMessage(msg.DeviceId, titem);
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
                    await DownUpVideoItemMessage(msg.DeviceId, tlist[0]);
                }
            }
            else
            {
                return;
            }


        }

        public virtual async Task TimerClean()
        {
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
                    foreach (var recF in recFiles)
                    {
                        await recordBLL.PublishCleanRecordMessage(recF.NodeId, recF.VideoId, recF.VideoKey, recF.StorageWay.Value, recF.FileDate.Value, recF.FileName);
                    }

                    ++i;
                }
                records = await recordDAL.GetUnCleanRecords(minDate, 1000);
            }
        }
    }
}
