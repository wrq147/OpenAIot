using Common.EventBus;
using Microsoft.Extensions.Logging;
using MonitorService.Util;
using Quartz;
using ReportService.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace ReportService.TimerUtil
{
    [DisallowConcurrentExecution]
    public class TimerShareConcurrentJob : IJob
    {
        private ILogger<TimerShareConcurrentJob> _log;
        private ITAServiceProvider _provider;
        public TimerShareConcurrentJob(ITAServiceProvider provider, ILoggerFactory factory)
        {
            _provider = provider;
            _log = factory.CreateLogger<TimerShareConcurrentJob>();
        }
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                string warnId = context.JobDetail.JobDataMap.GetString("PlanId");
                QuartzContext quartzContext = new QuartzContext();
                quartzContext.PreviousFireTimeUtc = context.PreviousFireTimeUtc;
                quartzContext.ScheduledFireTimeUtc = context.ScheduledFireTimeUtc;
                await _provider.GetService<ShareBLL>().Execute(warnId, quartzContext);
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
            }
        }
    }
}
