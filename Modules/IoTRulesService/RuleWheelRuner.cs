using IoTService;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using TemplateAction.Cache;
using TemplateAction.Core;

namespace IoTRulesService
{
    /// <summary>
    /// 规则引擎执行者
    /// </summary>
    public class RuleWheelRuner
    {
        List<HashedWheelTimer> _schedulers;
        ITAServiceProvider _provider;
        ILogger<RuleWheelRuner> _log;
        public RuleWheelRuner(ITAServiceProvider provider, ILoggerFactory logfactory)
        {
            _log = logfactory.CreateLogger<RuleWheelRuner>();
            _provider = provider;
            var option = _provider.GetService<IOptions<IotOption>>();
            int totalcc = 4;
            if (option.Value.runer_count > 0)
            {
                totalcc = option.Value.runer_count;
            }
            _schedulers = new List<HashedWheelTimer>();
            for (int i = 0; i < totalcc; i++)
            {
                var tmpsche = new HashedWheelTimer(TimeSpan.FromMilliseconds(100), 1024, 0);
                _schedulers.Add(tmpsche);
            }

        }
        public void PushConcurrentTask(string key, Action ac)
        {
            PushConcurrentTask(key, ac, TimeSpan.Zero);
        }
        public void PushConcurrentTask(string key, Action ac, TimeSpan ts)
        {
            int curidx = Math.Abs(key.GetHashCode() % _schedulers.Count);
            _schedulers[curidx].NewTimeout(new RuleTask(ac), ts);
        }
    }

    public class RuleTask : TimerTask
    {
        private Action _ac;
        public RuleTask(Action ac)
        {
            _ac = ac;
        }
        public void Run(IWheelTimeout timeout)
        {
            _ac();
        }
    }
}
