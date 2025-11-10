using Quartz;
using Quartz.Spi;
using System;
using TemplateAction.Core;

namespace MonitorService.Util
{
    /// <summary>
    /// 控制反转的Job创建工厂
    /// </summary>
    public class IOCJobFactory : IJobFactory
    {
        private ITAServiceProvider _provider;
        public IOCJobFactory(ITAServiceProvider provider)
        {
            _provider = provider;
        }
        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return _provider.GetService(bundle.JobDetail.JobType) as IJob;
        }

        public void ReturnJob(IJob job)
        {
            var disposable = job as IDisposable;
            disposable?.Dispose();
        }
    }
}
