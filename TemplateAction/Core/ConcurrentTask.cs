using System;
using TemplateAction.Cache;

namespace TemplateAction.Core
{
    /// <summary>
    /// 同步任务
    /// </summary>
    public class ConcurrentTask : TimerTask
    {
        private Action _ac;
        public ConcurrentTask(Action ac)
        {
            _ac = ac;
        }
        public void Run(IWheelTimeout timeout)
        {
            _ac?.Invoke();
        }
    }
}
