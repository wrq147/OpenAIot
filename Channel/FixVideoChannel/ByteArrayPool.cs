using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixVideoChannel
{
    public class ByteArrayPool
    {
        // 按大小分桶缓存，避免重复分配/GC
        private ConcurrentDictionary<int, ConcurrentQueue<byte[]>> _pool = new();

        /// <summary>
        /// 申请精确长度的字节数组
        /// </summary>
        public byte[] Rent(int size)
        {
            if (size <= 0)
                throw new ArgumentOutOfRangeException(nameof(size));

            var queue = _pool.GetOrAdd(size, static _ => new ConcurrentQueue<byte[]>());

            if (queue.TryDequeue(out var buffer))
            {
                return buffer;
            }

            // 精确分配
            return new byte[size];
        }

        /// <summary>
        /// 归还（必须和 Rent 大小一致）
        /// </summary>
        public void Return(byte[] buffer)
        {
            if (buffer == null) return;

            int size = buffer.Length;

            var queue = _pool.GetOrAdd(size, static _ => new ConcurrentQueue<byte[]>());
            queue.Enqueue(buffer);
        }


    }
}
