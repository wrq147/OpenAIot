using Common.Json;
using Microsoft.Extensions.Logging;
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
            string planId = context.JobDetail.JobDataMap.GetString("PlanId");
        }
    }
}
