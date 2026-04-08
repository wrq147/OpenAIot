using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace GB28181Channel
{
    public class ByteArrayPool : IDisposable
    {
        // 缓存池
        private readonly ConcurrentDictionary<int, ConcurrentQueue<byte[]>> _pool = new();

        private class Counter
        {
            public long Current;
        }
        private readonly ConcurrentDictionary<int, Counter> _counters = new();

        private readonly Timer _cleanTimer;
        private const int ReserveExtra = 10;
        private const int MinRetain = 2;
        private static readonly TimeSpan CleanInterval = TimeSpan.FromSeconds(10);

        public ByteArrayPool()
        {
            _cleanTimer = new Timer(CleanExcessBuffers, null, CleanInterval, CleanInterval);
        }

        /// <summary>
        /// 申请数组
        /// </summary>
        public byte[] Rent(int size)
        {
            if (size <= 0)
                throw new ArgumentOutOfRangeException(nameof(size));

            var queue = _pool.GetOrAdd(size, static _ => new ConcurrentQueue<byte[]>());
            queue.TryDequeue(out var buffer);

            // 原子计数+1，更新峰值
            var counter = _counters.GetOrAdd(size, static _ => new Counter());
            Interlocked.Increment(ref counter.Current);

            return buffer ?? new byte[size];
        }

        /// <summary>
        /// 归还数组（计数-1）
        /// </summary>
        public void Return(byte[] buffer)
        {
            if (buffer == null) return;
            int size = buffer.Length;

            // 原子-1，无报错
            var counter = _counters.GetOrAdd(size, static _ => new Counter());
            Interlocked.Decrement(ref counter.Current);

            // 归还队列
            var queue = _pool.GetOrAdd(size, static _ => new ConcurrentQueue<byte[]>());
            queue.Enqueue(buffer);
        }


        /// <summary>
        /// 清理多余缓存
        /// </summary>
        private void CleanExcessBuffers(object state)
        {
            foreach (int size in _pool.Keys.ToList())
            {
                try
                {
                    if (_counters.TryGetValue(size, out var counter))
                    {
                        int maxRetain = (int)counter.Current + ReserveExtra;
                        maxRetain = Math.Max(maxRetain, MinRetain);

                        if (_pool.TryGetValue(size, out var queue))
                        {
                            while (queue.Count > maxRetain)
                                queue.TryDequeue(out _);
                        }
                    }
                }
                catch { }
            }
        }

        public void Dispose()
        {
            _cleanTimer?.Change(Timeout.Infinite, Timeout.Infinite);
            _cleanTimer?.Dispose();
            _pool.Clear();
            _counters.Clear();
        }
    }
}
