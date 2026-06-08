using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TemplateAction.Cache;
using TemplateAction.Common;
using TemplateAction.Core;

namespace IoTAIService
{
    public class AITaskRuner
    {
        List<HashedWheelTimer> _schedulers;
        ITAServiceProvider _provider;
        ILogger<AITaskRuner> _log;
        public AITaskRuner(ITAServiceProvider provider, ILoggerFactory logfactory)
        {
            _log = logfactory.CreateLogger<AITaskRuner>();
            _provider = provider;
            var option = _provider.GetService<IOptions<IoTAIOption>>();
            int totalcc = 4;
            if (option.Value.RunerCount > 0)
            {
                totalcc = option.Value.RunerCount;
            }
            _schedulers = new List<HashedWheelTimer>();
            for (int i = 0; i < totalcc; i++)
            {
                var tmpsche = new HashedWheelTimer(TimeSpan.FromMilliseconds(100), 1024, 0);
                _schedulers.Add(tmpsche);
            }

        }
        public void PushConcurrentTask(string key, Func<Task> ac)
        {
            int curidx = Math.Abs(key.GetHashCode() % _schedulers.Count);
            _schedulers[curidx].NewTimeout(new AITask(ac), TimeSpan.Zero);
        }
    }
    public class AITask : TimerTask
    {
        private Func<Task> _ac;
        public AITask(Func<Task> ac)
        {
            _ac = ac;
        }
        public void Run(IWheelTimeout timeout)
        {
            TAAsyncHelper.RunSync(_ac);
        }
    }
}
