using AuthService;
using ChannelUtility.Message;
using Common.EventBus;
using Common.IdGenerator;
using Common.Share;
using InfluxDB.Client.Api.Domain;
using IoTService.DAL;
using IoTVideoService.DAL;
using IoTVideoService.Models;
using IoTVideoService.PlanUtil;
using MonitorService.Business;
using MonitorService.Model;
using MyAccess.DB.Builder.WhereToSql;
using Quartz;
using System;
using System.Collections.Generic;
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
        public async Task<BusResponse<string>> PublishStartRecordMessage(string nodeGuid, string streamId, string sourceId, string channelId)
        {
            MediaRecordStartMessage msg = new MediaRecordStartMessage();
            msg.DeviceId = sourceId;
            msg.ProductId = string.Empty;
            msg.ChannelId = channelId;
            msg.StreamId = streamId;
            msg.MessageId = Guid.NewGuid().ToString("N");

            var replyMsg = await _provider.GetService<NatsScope>().PublicWait<MediaRecordStartMessageReply>(nodeGuid, msg);
            if (replyMsg == null)
            {
                return BusResponse<string>.Error(112, "开始录制命令无回复");
            }
            if (replyMsg.IsSuccess)
            {
                return BusResponse<string>.Success();
            }
            else
            {
                return BusResponse<string>.Error(113, replyMsg.Reason);
            }
        }
        public async Task<BusResponse<string>> PublishStopRecordMessage(string nodeGuid, string streamId, string sourceId, string channelId)
        {
            MediaRecordStopMessage msg = new MediaRecordStopMessage();
            msg.DeviceId = sourceId;
            msg.ProductId = string.Empty;
            msg.ChannelId = channelId;
            msg.StreamId = streamId;
            msg.MessageId = Guid.NewGuid().ToString("N");

            var replyMsg = await _provider.GetService<NatsScope>().PublicWait<MediaRecordStopMessageReply>(nodeGuid, msg);
            if (replyMsg == null)
            {
                return BusResponse<string>.Error(112, "停止录制命令无回复");
            }
            if (replyMsg.IsSuccess)
            {
                return BusResponse<string>.Success();
            }
            else
            {
                return BusResponse<string>.Error(113, replyMsg.Reason);
            }
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
            var rsp = await _provider.GetService<RecordLogDAL>().SelectPage(expression, query, string.Empty);
            return rsp;
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
            }
            var recordDAL = _provider.GetService<RecordDAL>();
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
            if (data.Status != null)
            {
                if (data.Status == 0 && old.Status != 0)
                {
                    var schedulerFactory = _provider.GetService<ISchedulerFactory>();
                    var scheduler = await schedulerFactory.GetScheduler();
                    await PlanSchedule.DeleteJob(data.Id);
                }
                else if (data.Status == 1 && old.Status != 1)
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
