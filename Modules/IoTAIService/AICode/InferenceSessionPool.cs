using Microsoft.ML.OnnxRuntime;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IoTAIService.AICode
{
    public class InferenceSessionPool : IDisposable
    {
        #region 单例
        private static readonly Lazy<InferenceSessionPool> _instance = new(() => new InferenceSessionPool());
        public static InferenceSessionPool Instance => _instance.Value;
        #endregion

        #region 配置
        private const int SessionIdleTimeoutMinutes = 30;
        private const int CleanupIntervalMinutes = 10;
        #endregion

        #region 线程安全结构
        private readonly ConcurrentDictionary<string, ConcurrentQueue<SessionWrapper>> _sessionPool = new();
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(); // 读写锁
        private readonly Timer _cleanupTimer;
        private bool _disposed = false;
        #endregion

        private InferenceSessionPool()
        {
            _cleanupTimer = new Timer(CleanupIdleSessions, null,
                TimeSpan.FromMinutes(CleanupIntervalMinutes),
                TimeSpan.FromMinutes(CleanupIntervalMinutes));
        }

        #region 获取会话（读锁）
        public InferenceSession GetInferenceSession(string type, Func<InferenceSession> createSessionFunc)
        {
            if (string.IsNullOrEmpty(type)) throw new ArgumentNullException(nameof(type));
            if (createSessionFunc == null) throw new ArgumentNullException(nameof(createSessionFunc));

            _lock.EnterReadLock();
            try
            {
                if (_sessionPool.TryGetValue(type, out var queue) && queue.TryDequeue(out var wrapper))
                {
                    wrapper.LastUseTime = DateTime.Now;
                    return wrapper.Session;
                }
                return createSessionFunc();
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }
        #endregion

        #region 释放会话（读锁）
        public void ReleaseSession(string type, InferenceSession session)
        {
            if (string.IsNullOrEmpty(type) || session == null) return;

            _lock.EnterReadLock();
            try
            {
                var queue = _sessionPool.GetOrAdd(type, _ => new ConcurrentQueue<SessionWrapper>());
                queue.Enqueue(new SessionWrapper(session, DateTime.Now));
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }
        #endregion

        #region 清理空闲会话（写锁 + 不出列 + 安全遍历）
        private void CleanupIdleSessions(object _)
        {
            if (_disposed) return;

            try
            {
                var expireTime = DateTime.Now.AddMinutes(-SessionIdleTimeoutMinutes);

                _lock.EnterWriteLock();
                try
                {
                    foreach (var modelType in _sessionPool.Keys.ToList())
                    {
                        if (!_sessionPool.TryGetValue(modelType, out var sessionQueue))
                            continue;

                        // 核心：Peek 判定 → 过期才出列
                        while (sessionQueue.TryPeek(out var wrapper) && wrapper.LastUseTime < expireTime)
                        {
                            // 确认过期 → 安全出列
                            if (sessionQueue.TryDequeue(out var expiredWrapper))
                            {
                                try { expiredWrapper.Session.Dispose(); }
                                catch { }
                            }
                        }

                        // 空队列移除类型
                        if (sessionQueue.IsEmpty)
                            _sessionPool.TryRemove(modelType, out var _);
                    }
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[会话池清理异常] {ex}");
            }
        }
        #endregion

        #region 会话包装
        private class SessionWrapper
        {
            public InferenceSession Session { get; }
            public DateTime LastUseTime { get; set; }
            public SessionWrapper(InferenceSession session, DateTime lastUseTime)
            {
                Session = session;
                LastUseTime = lastUseTime;
            }
        }
        #endregion

        #region 释放
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                _cleanupTimer?.Dispose();
                _lock.EnterWriteLock();
                try
                {
                    foreach (var queue in _sessionPool.Values)
                    {
                        foreach (var wrapper in queue.ToList())
                        {
                            wrapper.Session.Dispose();
                        }
                    }
                    _sessionPool.Clear();
                    _lock.Dispose();
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
            _disposed = true;
        }

        ~InferenceSessionPool() => Dispose(false);
        #endregion
    }
}
