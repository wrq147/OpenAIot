using System;
using System.Threading;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace TemplateAction.Cache
{
    /// <summary>
    /// 缓存池
    /// </summary>
    public class CachePool
    {
        int _maxsize;
        ConcurrentDictionary<string, CacheItem> _cache = new ConcurrentDictionary<string, CacheItem>();
        LinkedList<string> _link;
        HashedWheelTimer _scheduler;
        SpinLock _spinlock;
        Timer _clearTimer;
        public CachePool(int maxsize, bool schedule)
        {
            _maxsize = maxsize;
            if (schedule)
            {
                _scheduler = new HashedWheelTimer(TimeSpan.FromSeconds(1), 1024, 0);
            }
            if (_maxsize > 0)
            {
                _link = new LinkedList<string>();
                _spinlock = new SpinLock();
            }
            _clearTimer = new Timer(new TimerCallback(MonitorClear), null, 0, 180000);
        }
        public CachePool(bool schedule) : this(-1, schedule) { }
        public CachePool(int maxsize) : this(maxsize, true) { }
        public CachePool() : this(-1, false) { }
        private void MonitorClear(object state)
        {
            Clear();
        }
        private int GetLKCount()
        {
            bool gotLock = false;
            try
            {
                _spinlock.Enter(ref gotLock);//加锁
                return _link.Count;
            }
            finally
            {
                if (gotLock) _spinlock.Exit();//解锁
            }
        }
        private void RemoveNode(LinkedListNode<string> n)
        {
            bool gotLock = false;
            try
            {
                _spinlock.Enter(ref gotLock);//加锁
                if (n.List == _link)
                {
                    _link.Remove(n);
                }
            }
            finally
            {
                if (gotLock) _spinlock.Exit();//解锁
            }
        }
        private void AddFirstNode(LinkedListNode<string> n)
        {
            bool gotLock = false;
            try
            {
                _spinlock.Enter(ref gotLock);//加锁
                if (n.List == _link)
                {
                    _link.AddFirst(n);
                }
            }
            finally
            {
                if (gotLock) _spinlock.Exit();//解锁
            }
        }
        public void Remove(string key)
        {
            CacheItem item;
            if (_cache.TryRemove(key, out item))
            {
                if (_maxsize > 0)
                {
                    RemoveNode(item.Node);
                }
                item.Dependency.Dispose(item);
            }
        }
        /// <summary>
        /// 清除无效项
        /// </summary>
        public void Clear()
        {
            foreach (KeyValuePair<string, CacheItem> kvp in _cache)
            {
                CacheItem tval = kvp.Value;
                if (!Equals(tval.Dependency, null))
                {
                    if (tval.Dependency.Validate())
                    {
                        Remove(tval.Key);
                    }
                }
            }
        }
        public void Empty()
        {
            foreach (KeyValuePair<string, CacheItem> kvp in _cache)
            {
                CacheItem tval = kvp.Value;
                Remove(tval.Key);
            }
        }
        public object Get(string key)
        {
            CacheItem val;
            if (_cache.TryGetValue(key, out val))
            {
                bool isnew = false;
                if (!Equals(val.Dependency, null))
                {
                    isnew = val.Dependency.Validate();
                }
                if (isnew)
                {
                    Remove(key);
                    return null;
                }
                if (_maxsize > 0)
                {
                    RemoveNode(val.Node);
                    AddFirstNode(val.Node);
                }
                return val.Value;
            }
            else
            {
                return null;
            }
        }
        public void Insert(string key, object value, IDependency dependency)
        {
            CacheItem ci = new CacheItem();
            ci.Key = key;
            ci.Value = value;
            ci.Dependency = dependency;
            if (_cache.TryAdd(key, ci))
            {
                if (!Equals(dependency, null))
                {
                    dependency.Init(this, key);
                }
                if (_maxsize > 0)
                {
                    ci.Node = new LinkedListNode<string>(key);
                    AddFirstNode(ci.Node);
                    if (GetLKCount() > _maxsize)
                    {
                        Remove(_link.Last.Value);
                    }
                }
            }
        }

        public void AddSchedule(string key, DateTime end)
        {
            TimeOutSchedule newtoi = new TimeOutSchedule();
            newtoi.Pool = this;
            newtoi.ItemKey = key;
            _scheduler.NewTimeout(newtoi, end - DateTime.Now);
        }
    }
}
