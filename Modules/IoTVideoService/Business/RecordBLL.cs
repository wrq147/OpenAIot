using AuthService;
using ChannelUtility.Message;
using Common.EventBus;
using Common.IdGenerator;
using Common.Share;
using IoTVideoService.DAL;
using IoTVideoService.Models;
using IoTVideoService.PlanUtil;
using Microsoft.Extensions.Options;
using MyAccess.DB.Builder.WhereToSql;
using NodaTime;
using NPOI.HPSF;
using Quartz;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace IoTVideoService.Business
{
    public class RecordBLL
    {
        private ITAServiceProvider _provider;
        public RecordBLL(ITAServiceProvider provider)
        {
            _provider = provider;
        }
        public async Task InsertRecordFile(MediaRecordFileMessage msg)
        {
            var recordDAL = _provider.GetService<RecordDAL>();
            var rec = (await recordDAL.SelectList(x => x.VideoId == msg.DeviceId)).FirstOrDefault();
            if (rec == null)
            {
                return;
            }
            var snowflake = _provider.GetService<SnowflakeHelper>();
            MZ_IotRecordFile recFile = new MZ_IotRecordFile();
            recFile.Id = snowflake.NextId().ToString();
            recFile.StartTime = DateTimeOffset.FromUnixTimeSeconds((long)msg.StartTime).LocalDateTime;
            recFile.FileDate = recFile.StartTime.Value.Date;
            recFile.PlanId = rec.Id;
            recFile.VideoKey = msg.StreamId;
            recFile.VideoId = rec.VideoId;
            recFile.NodeId = msg.NodeId;
            recFile.StorageWay = msg.Storage;
            recFile.FileName = msg.FileName;
            recFile.SaveType = msg.SaveType;
            recFile.FileSize = (float?)(msg.FileSize / (1024.0 * 1024.0));
            long endlong = (long)msg.StartTime + (long)msg.TimeLen;
            recFile.EndTime = DateTimeOffset.FromUnixTimeSeconds(endlong).LocalDateTime;
            await _provider.GetService<RecordFileDAL>().Insert(recFile);
        }
        public async Task PublishCleanRecordMessage(string nodeId, string videoId, List<MZ_IotRecordFile> records)
        {
            MediaRecordCleanMessage msg = new MediaRecordCleanMessage();
            msg.DeviceId = videoId;
            msg.ProductId = string.Empty;
            List<FileRecord> rrss = new List<FileRecord>();
            foreach (var record in records)
            {
                rrss.Add(new FileRecord()
                {
                    Storage = record.StorageWay.Value,
                    StreamId = record.VideoKey,
                    Date = record.FileDate.Value.ToString("yyyy-MM-dd"),
                    FileName = record.FileName
                });
            }
            msg.Records = rrss;
            await _provider.GetService<NatsScope>().Public(nodeId, msg);
        }
        public async Task<BusResponse<string>> PublishStartRecordMessage(string planId, byte storageWay, DateTime time, MZ_VideoSource source)
        {
            string nodeId = null;
            if (!string.IsNullOrEmpty(source.NodeId))
            {
                nodeId = source.NodeId;
            }
            else
            {
                if (source.VideoType == 0)
                {
                    var option = _provider.GetService<IOptions<VideoOption>>();
                    if (option.Value.VideoServers == null || option.Value.VideoServers.Count == 0)
                    {
                        return BusResponse<string>.Error(211, "VideoOption配置错误");
                    }
                    int pos = Math.Abs(source.Id.GetHashCode() % option.Value.VideoServers.Count);
                    ServerInfo serverInfo = option.Value.VideoServers[pos];
                    nodeId = serverInfo.NodeId;
                }
                else if (source.VideoType == 1)
                {
                    return BusResponse<string>.Error(212, "视频源未注册");
                }
                else
                {
                    return BusResponse<string>.Error(220, "录像的视频源类型错误");
                }
            }
            List<MZ_VideoSource> sourceList = new List<MZ_VideoSource>();
            if (source.VideoType == 1)
            {
                var channelList = await _provider.GetService<VideoSourceDAL>().SelectList(x => x.VideoType == 2 && x.UserName == source.UserName);
                sourceList.AddRange(channelList);
            }
            else
            {
                sourceList.Add(source);
            }

            var snowflake = _provider.GetService<SnowflakeHelper>();
            foreach (var recSource in sourceList)
            {

                MediaRecordStartMessage msg = new MediaRecordStartMessage();
                msg.DeviceId = source.Id;
                msg.ProductId = string.Empty;
                msg.StreamId = recSource.VideoKey;
                msg.Storage = storageWay;
                msg.SaveType = 1;
                msg.MessageId = Guid.NewGuid().ToString("N");

                var replyMsg = await _provider.GetService<NatsScope>().PublicWait<MediaRecordStartMessageReply>(nodeId, msg);
                if (replyMsg == null || !replyMsg.IsSuccess)
                {
                    MZ_IotRecordLog log = new MZ_IotRecordLog();
                    log.Id = snowflake.NextId().ToString();
                    log.PlanId = planId;
                    log.Position = source.Position + "-" + recSource.Position;
                    log.VideoId = source.Id;
                    log.LogType = "fail";
                    log.Content = replyMsg == null ? "录制命令无回复" : replyMsg.Reason;
                    log.ExecTime = DateTime.Now;
                    await _provider.GetService<RecordLogDAL>().Insert(log);
                }
                else
                {
                    MZ_IotRecordLog log = new MZ_IotRecordLog();
                    log.Id = snowflake.NextId().ToString();
                    log.PlanId = planId;
                    log.Position = source.Position + "-" + recSource.Position;
                    log.VideoId = source.Id;
                    log.LogType = "start";
                    log.Content = string.Empty;
                    log.ExecTime = DateTime.Now;
                    await _provider.GetService<RecordLogDAL>().Insert(log);
                }
            }

            return BusResponse<string>.Success();
        }
        public async Task<BusResponse<string>> PublishStopRecordMessage(string planId, MZ_IotRecord record, MZ_VideoSource source)
        {
            var recFileDAL = _provider.GetService<RecordFileDAL>();
            var recDAL = _provider.GetService<RecordDAL>();
            var sourceDAL = _provider.GetService<VideoSourceDAL>();

            if (string.IsNullOrEmpty(source.NodeId))
            {
                return BusResponse<string>.Error(113, "视频源未注册");
            }
            string nodeId = source.NodeId;

            List<MZ_VideoSource> sourceList = new List<MZ_VideoSource>();
            if (source.VideoType == 1)
            {
                var channelList = await sourceDAL.SelectList(x => x.VideoType == 2 && x.UserName == source.UserName);
                sourceList.AddRange(channelList);
            }
            else
            {
                sourceList.Add(source);
            }

            var snowflake = _provider.GetService<SnowflakeHelper>();
            foreach (var recSource in sourceList)
            {
                MediaRecordStopMessage msg = new MediaRecordStopMessage();
                msg.DeviceId = source.Id;
                msg.ProductId = string.Empty;
                msg.StreamId = recSource.VideoKey;
                msg.SaveType = 1;
                msg.MessageId = Guid.NewGuid().ToString("N");

                var replyMsg = await _provider.GetService<NatsScope>().PublicWait<MediaRecordStopMessageReply>(nodeId, msg);
                if (replyMsg == null || !replyMsg.IsSuccess)
                {
                    MZ_IotRecordLog log = new MZ_IotRecordLog();
                    log.Id = snowflake.NextId().ToString();
                    log.PlanId = planId;
                    log.Position = source.Position + "-" + recSource.Position;
                    log.VideoId = source.Id;
                    log.LogType = "fail";
                    log.Content = replyMsg == null ? "录制命令无回复" : replyMsg.Reason;
                    log.ExecTime = DateTime.Now;
                    await _provider.GetService<RecordLogDAL>().Insert(log);
                }
                else
                {
                    MZ_IotRecordLog log = new MZ_IotRecordLog();
                    log.Id = snowflake.NextId().ToString();
                    log.PlanId = planId;
                    log.Position = source.Position + "-" + recSource.Position;
                    log.VideoId = source.Id;
                    log.LogType = "stop";
                    log.Content = string.Empty;
                    log.ExecTime = DateTime.Now;
                    await _provider.GetService<RecordLogDAL>().Insert(log);
                }
            }
            return BusResponse<string>.Success();
        }

        public virtual async Task<List<MZ_IotRecordKey>> SelectKeyList(In_RecordKeyList query)
        {
            return await _provider.GetService<RecordKeyDAL>().SelectList(x => x.VideoKey == query.Key && x.KeyDate >= query.beginTime && x.KeyDate < query.endTime);
        }
        public virtual async Task<PageObject<MZ_IotRecord>> SelectPage(In_RecordPage query, IUserInfo user)
        {
            Expression<Func<MZ_IotRecord, bool>> expression = x => x.OrgId == user.OrgId;
            if (!string.IsNullOrEmpty(query.VideoId))
            {
                expression = expression.And(x => x.VideoId.StartsWith(query.VideoId));
            }
            if (!string.IsNullOrEmpty(query.RecordTimeType))
            {
                expression = expression.And(x => x.RecordTimeType == query.RecordTimeType);
            }
            if (query.Status != null)
            {
                expression = expression.And(x => x.Status == query.Status);
            }
            var rsp = await _provider.GetService<RecordDAL>().SelectPage(expression, query, string.Empty);
            return rsp;
        }

        public virtual async Task<PageObject<MZ_IotRecordLog>> SelectLogPage(In_RecordLogPage query)
        {
            Expression<Func<MZ_IotRecordLog, bool>> expression = x => x.PlanId == query.PlanId;
            if (!string.IsNullOrEmpty(query.LogType))
            {
                expression = expression.And(x => x.LogType == query.LogType);
            }
            var rsp = await _provider.GetService<RecordLogDAL>().SelectPage(expression, query, "ExecTime desc");
            return rsp;
        }
        public virtual async Task<PageObject<MZ_IotRecordFile>> SelecFilePage(In_RecordFilePage query)
        {
            Expression<Func<MZ_IotRecordFile, bool>> expression;
            if (!string.IsNullOrEmpty(query.PlanId))
            {
                expression = x => x.PlanId == query.PlanId;
            }
            else if (!string.IsNullOrEmpty(query.VideoId))
            {
                expression = x => x.VideoId == query.VideoId;
            }
            else
            {
                return PageObject<MZ_IotRecordFile>.Empty();
            }
            if (!string.IsNullOrEmpty(query.VideoKey))
            {
                expression = expression.And(x => x.VideoKey == query.VideoKey);
            }
            if (query.beginTime != null)
            {
                expression = expression.And(x => x.FileDate >= query.beginTime);
            }
            if (query.endTime != null)
            {
                expression = expression.And(x => x.FileDate <= query.endTime);
            }
            var rsp = await _provider.GetService<RecordFileDAL>().SelectPage(expression, query, "StartTime desc");
            foreach (var item in rsp.List)
            {
                item.PlayUrl = GeneratePlayUrl(item);
            }
            return rsp;
        }
        private string GeneratePlayUrl(MZ_IotRecordFile file)
        {
            if (file.StorageWay == 0)
            {
                var option = _provider.GetService<IOptions<VideoOption>>();
                if (option.Value.VideoServers.Count == 0 && option.Value.GB28181Servers.Count == 0)
                {
                    return string.Empty;
                }

                var curNode = option.Value.VideoServers.Where(x => x.NodeId == file.NodeId).FirstOrDefault();
                if (curNode == null)
                {
                    curNode = option.Value.GB28181Servers.Where(x => x.NodeId == file.NodeId).FirstOrDefault();
                }
                if (curNode == null)
                {
                    return string.Empty;
                }
                return $"http://{curNode.Ip}:{curNode.HttpPort}/record/live/{file.VideoKey}/{file.FileDate.Value.ToString("yyyy-MM-dd")}/{file.FileName}";
            }
            else if (file.StorageWay == 1)
            {
                var option = _provider.GetService<IOptions<VideoOption>>();
                string upfilePosition = $"{file.VideoKey}/{file.FileDate.Value.ToString("yyyy-MM-dd")}/{file.FileName}";
                return $"{option.Value.minio_url}/{option.Value.minio_bucket}/{upfilePosition}";
            }
            else
            {
                return string.Empty;
            }
        }
        public virtual async Task<BusResponse<MZ_IotRecord>> Info(string id)
        {
            var recordDAL = _provider.GetService<RecordDAL>();
            var info = await recordDAL.Select(id);
            if (info == null)
            {
                return BusResponse<MZ_IotRecord>.Error(111, "录像计划不存在");
            }
            if (info.createId > 0)
            {
                var creator = await _provider.GetService<UserDAL>().Select(info.createId);
                if (creator != null)
                {
                    info.createName = creator.RealName;
                }
            }
            return BusResponse<MZ_IotRecord>.Success(info);
        }
        public virtual async Task<BusResponse<int>> Insert(MZ_IotRecord data, IUserInfo user)
        {
            if (user.OrgId <= 0)
            {
                return BusResponse<int>.Error(112, "非企业用户无法添加");
            }
            var videoSourceDAL = _provider.GetService<VideoSourceDAL>();
            var videoInfo = await videoSourceDAL.Select(data.VideoId);
            if (videoInfo == null)
            {
                return BusResponse<int>.Error(113, "视频源不存在");
            }
            var recordDAL = _provider.GetService<RecordDAL>();
            if (await recordDAL.Some(x => x.VideoId == data.VideoId))
            {
                return BusResponse<int>.Error(114, "视频源已存在录像计划");
            }
            var snowflake = _provider.GetService<SnowflakeHelper>();
            data.Id = snowflake.NextId().ToString();
            data.OrgId = user.OrgId;
            data.Status = 1;
            data.Position = videoInfo.Position;
            data.SetCreateBy(user);

            var tasks = PlanTimeParser.GenerateTriggerTasks(data);
            if (tasks.Count > 0)
            {
                data.RecordTimeDesc = RecordTimeDescGenerator.GenerateTimeDesc(data.RecordTimeType, data.WeekConfig, data.TimeConfig);
                var schedulerFactory = _provider.GetService<ISchedulerFactory>();
                var scheduler = await schedulerFactory.GetScheduler();
                await PlanSchedule.CreateJob(data.Id, tasks);

                if (PlanTimeParser.GetTodayNextTriggerTask(tasks) != null)
                {
                    //直接开始录像
                    await this.PublishStartRecordMessage(data.Id, data.StorageWay.Value, DateTime.Now, videoInfo);
                }
            }

            return BusResponse<int>.Success(await recordDAL.Insert(data));
        }

        public virtual async Task<BusResponse<int>> Update(MZ_IotRecord data, IUserInfo user)
        {
            if (user.OrgId <= 0)
            {
                return BusResponse<int>.Error(112, "非企业用户无法修改");
            }
            var recordDAL = _provider.GetService<RecordDAL>();
            var old = await recordDAL.Select(data.Id);
            if (old == null)
            {
                return BusResponse<int>.Error(113, "录像计划不存在");
            }
            if (old.OrgId != user.OrgId)
            {
                return BusResponse<int>.Error(114, "无权修改当前录像计划");
            }
            var videoSourceDAL = _provider.GetService<VideoSourceDAL>();
            var videoInfo = await videoSourceDAL.Select(old.VideoId);
            if (videoInfo == null)
            {
                return BusResponse<int>.Error(115, "无效的录像计划，视频源不存在");
            }
            data.VideoId = null;
            data.Position = videoInfo.Position;
            if (!string.IsNullOrEmpty(data.RecordTimeType) && !string.IsNullOrEmpty(data.WeekConfig) && !string.IsNullOrEmpty(data.TimeConfig))
            {
                data.RecordTimeDesc = RecordTimeDescGenerator.GenerateTimeDesc(data.RecordTimeType, data.WeekConfig, data.TimeConfig);
            }
            if (data.Status != null && data.Status == 0 && old.Status != 0)
            {
                var schedulerFactory = _provider.GetService<ISchedulerFactory>();
                var scheduler = await schedulerFactory.GetScheduler();
                await PlanSchedule.DeleteJob(data.Id);
            }
            else if ((data.Status != null && data.Status == 1 && old.Status != 1) || (data.TimeConfig != old.TimeConfig || data.WeekConfig != old.WeekConfig))
            {
                var schedulerFactory = _provider.GetService<ISchedulerFactory>();
                var scheduler = await schedulerFactory.GetScheduler();
                await PlanSchedule.DeleteJob(data.Id);
                MZ_IotRecord newrec = new MZ_IotRecord();
                newrec.RecordTimeType = data.RecordTimeType ?? old.RecordTimeType;
                newrec.TimeConfig = data.TimeConfig ?? old.TimeConfig;
                newrec.WeekConfig = data.WeekConfig ?? old.WeekConfig;
                var tasks = PlanTimeParser.GenerateTriggerTasks(newrec);
                if (tasks.Count > 0)
                {
                    await PlanSchedule.CreateJob(data.Id, tasks);
                    if (PlanTimeParser.GetTodayNextTriggerTask(tasks) != null)
                    {
                        //直接开始录像
                        byte? storageWay = data.StorageWay ?? old.StorageWay;
                        await this.PublishStartRecordMessage(data.Id, storageWay.Value, DateTime.Now, videoInfo);
                    }
                }
            }

            data.SetUpdateBy(user);

            return BusResponse<int>.Success(await recordDAL.Update(data));
        }

        public virtual async Task<BusResponse<int>> Delete(string id, IUserInfo user)
        {
            if (user.OrgId <= 0)
            {
                return BusResponse<int>.Error(112, "非企业用户无法删除");
            }
            try
            {
                var recordDAL = _provider.GetService<RecordDAL>();
                var info = await recordDAL.Select(id);
                if (info == null)
                {
                    return BusResponse<int>.Error(111, "计划不存在");
                }

                var schedulerFactory = _provider.GetService<ISchedulerFactory>();
                var scheduler = await schedulerFactory.GetScheduler();
                await PlanSchedule.DeleteJob(id);

                var logDAL = _provider.GetService<RecordLogDAL>();
                await logDAL.Delete(x => x.PlanId == id);

                var fileDAL = _provider.GetService<RecordFileDAL>();
                await fileDAL.Delete(x => x.PlanId == id);

                var rs = await recordDAL.Delete(x => x.OrgId == user.OrgId && x.Id == id);
                return BusResponse<int>.Success(rs);
            }
            catch (Exception ex)
            {
                return BusResponse<int>.Error(111, ex.Message);
            }
        }

    }
}
