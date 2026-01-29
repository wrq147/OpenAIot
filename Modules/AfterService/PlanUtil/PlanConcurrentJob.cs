using AfterService.Business;
using Microsoft.Extensions.Logging;
using MonitorService.Util;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace AfterService.PlanUtil
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
            try
            {
                var fireTime = context.ScheduledFireTimeUtc.Value.LocalDateTime;
                string planId = context.JobDetail.JobDataMap.GetString("PlanId");
                await _provider.GetService<DevPlaneBLL>().Execute(planId, fireTime);
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
            }
   
        }
    }
}
