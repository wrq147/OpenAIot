using Microsoft.Extensions.Logging;
using MonitorService.Model;
using Quartz;
using System;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace MonitorService.Util
{
    public class QuartzJob : AbstractQuartzJob
    {
        private ILogger<QuartzJob> _log;
        public QuartzJob(ITAServiceProvider provider, ILoggerFactory factory) : base(provider)
        {
            _log = factory.CreateLogger<QuartzJob>();
        }
        protected override async Task DoExecute(IJobExecutionContext context, MZ_Job job)
        {
            await ScheduleUtils.InvokeMethod(Provider, context, job);
        }
    }
}
