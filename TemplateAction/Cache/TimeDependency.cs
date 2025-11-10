using System;

namespace TemplateAction.Cache
{
    public class TimeDependency : IDependency
    {
        private DateTime _endTime;
        private Action<object> _callback;
        public TimeDependency(int exp_seconds, Action<object> callback = null)
        {
            _callback = callback;
            _endTime = DateTime.Now.AddSeconds(exp_seconds);
        }
        public void Dispose(CacheItem item)
        {
            if (_callback != null)
            {
                _callback?.Invoke(item.Value);
            }
        }
        public void Init(CachePool pool, string key)
        {
            if (pool != null && _callback != null)
            {
                pool.AddSchedule(key, _endTime);
            }
        }
        public bool Validate()
        {
            if (_endTime < DateTime.Now)
            {
                return true;
            }
            return false;
        }
    }
}
