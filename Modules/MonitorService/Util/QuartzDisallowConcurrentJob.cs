using Microsoft.Extensions.Logging;
using MonitorService.Model;
using Quartz;
using System;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace MonitorService.Util
{
    [DisallowConcurrentExecution]
    public class QuartzDisallowConcurrentJob : AbstractQuartzJob
    {
        private ILogger<QuartzDisallowConcurrentJob> _log;
        public QuartzDisallowConcurrentJob(ITAServiceProvider provider, ILoggerFactory factory) : base(provider)
        {
            _log = factory.CreateLogger<QuartzDisallowConcurrentJob>();
        }
        protected override async Task DoExecute(IJobExecutionContext context, MZ_Job job)
        {
            await ScheduleUtils.InvokeMethod(Provider,context, job);
        }
    }
}
