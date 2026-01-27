using Common.Json;
using IoTVideoService.Business;
using IoTVideoService.DAL;
using Microsoft.Extensions.Logging;
using Minio;
using MonitorService.Model;
using MonitorService.Util;
using Quartz;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace IoTVideoService.PlanUtil
{
    [DisallowConcurrentExecution]
    public class PlanConcurrentJob : IJob
    {
        private ILogger<QuartzDisallowConcurrentJob> _log;
        private ITAServiceProvider _provider;
        public PlanConcurrentJob(ITAServiceProvider provider, ILoggerFactory factory)
        {
            _provider = provider;
            _log = factory.CreateLogger<QuartzDisallowConcurrentJob>();
        }
        public async Task Execute(IJobExecutionContext context)
        {
            var fireTime = context.ScheduledFireTimeUtc.Value.LocalDateTime;
            string planId = context.JobDetail.JobDataMap.GetString("PlanId");
            string tdata = context.Trigger.JobDataMap.GetString("TData");
            var operType = Enum.Parse<RecordTimeOp>(tdata);
            switch (operType)
            {
                case RecordTimeOp.Start:
                    {
                        var record = await _provider.GetService<RecordDAL>().Select(planId);
                        if (record == null)
                        {
                            await PlanSchedule.DeleteJob(planId);
                            return;
                        }
                        var source = await _provider.GetService<VideoSourceDAL>().Select(record.VideoId);
                        if (source == null)
                        {
                            await PlanSchedule.DeleteJob(planId);
                            return;
                        }
                        var rs = await _provider.GetService<RecordBLL>().PublishStartRecordMessage(planId, record.StorageWay.Value, fireTime, source);
                        if (!rs.IsSuccess())
                        {
                            _log.LogError(rs.Message);
                            return;
                        }
                    }
                    break;
                case RecordTimeOp.End:
                    {
                        var rs = await _provider.GetService<RecordBLL>().PublishStopRecordMessage(planId, fireTime);
                        if (!rs.IsSuccess())
                        {
                            _log.LogError(rs.Message);
                            return;
                        }
                    }
                    break;
                case RecordTimeOp.Both:
                    {
                        var recordBLL = _provider.GetService<RecordBLL>();
                        var rs = await recordBLL.PublishStopRecordMessage(planId, fireTime);
                        if (!rs.IsSuccess())
                        {
                            _log.LogError(rs.Message);
                            return;
                        }

                        var record = await _provider.GetService<RecordDAL>().Select(planId);
                        if (record == null)
                        {
                            await PlanSchedule.DeleteJob(planId);
                            return;
                        }
                        var source = await _provider.GetService<VideoSourceDAL>().Select(record.VideoId);
                        if (source == null)
                        {
                            await PlanSchedule.DeleteJob(planId);
                            return;
                        }
                        rs = await recordBLL.PublishStartRecordMessage(planId, record.StorageWay.Value, fireTime, source);
                        if (!rs.IsSuccess())
                        {
                            _log.LogError(rs.Message);
                            return;
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
