using ChannelUtility.Message;
using Common.EventBus;
using Common.IdGenerator;
using Common.Share;
using IoTVideoService.DAL;
using IoTVideoService.Models;
using MonitorService.Business;
using MonitorService.Model;
using MyAccess.DB.Builder.WhereToSql;
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

        public virtual async Task<BusResponse<string>> Add(MZ_IotRecord data, IUserInfo user)
        {
            var snowflake = _provider.GetService<SnowflakeHelper>();
            data.Id = snowflake.NextId().ToString();
            data.OrgId = user.OrgId;
          
            //if (data.StartWay == 1)
            //{
            //    if (string.IsNullOrEmpty(data.TimerCron))
            //    {
            //        return BusResponse<string>.Error(131, "Cron表达式不能为空");
            //    }
            //    MZ_Job job = new MZ_Job();
            //    job.concurrent = "1";
            //    job.createId = 0;
            //    job.create_time = DateTime.Now;
            //    job.updateId = 0;
            //    job.update_time = DateTime.Now;
            //    job.cron_expression = data.TimerCron;
            //    job.invoke_target = typeof(DevPlaneBLL).FullName + ".Execute('" + data.Id + "',$id)";
            //    job.job_group = "DEFAULT";
            //    job.job_name = "DevPlaneTimer-" + data.Id;
            //    job.misfire_policy = "0";
            //    job.status = "0";

            //    var rs = await _provider.GetService<JobBLL>().InsertJob(job);
            //    if (!rs.IsSuccess())
            //    {
            //        return BusResponse<string>.Error(rs.Code, rs.Message);
            //    }

            //    data.TimerJobId = rs.Data;
            //}
            //else if (data.StartWay == 2)
            //{
            //    data.TimerCron = string.Empty;
            //    data.TimerJobId = 0;
            //    if (data.Events == null)
            //    {
            //        return BusResponse<string>.Error(122, "请选择设备事件");
            //    }
            //    foreach (var evt in data.Events)
            //    {
            //        evt.PlaneId = data.Id;
            //        evt.OrgId = data.OrgId;
            //    }
            //    await _devPlaneDAL.AddPlaneEvent(data.Events);
            //}
            //else
            //{
            //    data.TimerCron = string.Empty;
            //    data.TimerJobId = 0;
            //    data.FlowCreatedUserId = 0;
            //}
            //data.SetCreateBy(user);

            //foreach (var item in data.Targets)
            //{
            //    item.PlaneId = data.Id;
            //}
            //await _devPlaneDAL.AddPlaneDevice(data.Targets);
            //await _devPlaneDAL.Insert(data);
            return BusResponse<string>.Success();
        }
    }
}
