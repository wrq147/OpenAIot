using Common.EventBus;
using IoTRulesService.Business;
using Microsoft.Extensions.Logging;
using MonitorService.Util;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace IoTRulesService.TimerUtil
{
    [DisallowConcurrentExecution]
    public class TimerConcurrentJob : IJob
    {
        private ILogger<QuartzDisallowConcurrentJob> _log;
        private ITAServiceProvider _provider;
        public TimerConcurrentJob(ITAServiceProvider provider, ILoggerFactory factory)
        {
            _provider = provider;
            _log = factory.CreateLogger<QuartzDisallowConcurrentJob>();
        }
        public async Task Execute(IJobExecutionContext context)
        {
            long ruleId = context.JobDetail.JobDataMap.GetLong("PlanId");
            QuartzContext quartzContext = new QuartzContext();
            quartzContext.PreviousFireTimeUtc = context.PreviousFireTimeUtc;
            quartzContext.ScheduledFireTimeUtc = context.ScheduledFireTimeUtc;
            await _provider.GetService<RuleBLL>().Execute(ruleId, 2, quartzContext, null);
        }
    }
}
