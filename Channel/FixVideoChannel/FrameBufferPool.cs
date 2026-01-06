using System;
using System.Collections.Concurrent;
using System.Threading;

namespace FixVideoChannel
{

    /// <summary>
    /// 通用帧缓冲区缓存池
    /// 专为视频帧BGR24缓冲区优化，线程安全
    /// </summary>
    public static class FrameBufferPool
    {
        // 核心缓冲区池（存储可复用的byte数组）
        private static readonly ConcurrentBag<byte[]> _byteBufferPool = new ConcurrentBag<byte[]>();

        // 按视频Key缓存不同尺寸的缓冲区（记录尺寸+缓冲区关联）
        private static readonly ConcurrentDictionary<string, FrameBufferCacheItem> _keyedBufferCache =
            new ConcurrentDictionary<string, FrameBufferCacheItem>();

        // 最大缓存数量（防止内存过度占用）
        private const int MaxPoolCount = 100;

        /// <summary>
        /// 获取指定尺寸的RGB24缓冲区（按视频Key缓存）
        /// </summary>
        /// <param name="videoKey">视频唯一标识</param>
        /// <param name="width">帧宽度</param>
        /// <param name="height">帧高度</param>
        /// <param name="align">内存对齐值（默认32）</param>
        /// <returns>复用的byte数组缓冲区</returns>
        public static byte[] GetRgb24Buffer(string videoKey, int width, int height, int align = 32)
        {
            // 参数校验
            if (string.IsNullOrEmpty(videoKey))
                throw new ArgumentNullException(nameof(videoKey), "视频Key不能为空");
            if (width <= 0 || height <= 0)
                throw new ArgumentOutOfRangeException($"宽度和高度必须大于0，当前：width={width}, height={height}");

            // 计算对齐后的缓冲区总大小
            const int pixelSize = 3; // RGB24每个像素3字节
            int rawLineSize = width * pixelSize;
            int alignedLineSize = (rawLineSize + align - 1) & ~(align - 1);
            int totalSize = alignedLineSize * height;

            // 1. 优先从Key缓存中获取匹配尺寸的缓冲区
            if (_keyedBufferCache.TryGetValue(videoKey, out var cacheItem)
                && cacheItem.Width == width
                && cacheItem.Height == height
                && cacheItem.TotalSize == totalSize)
            {
                return cacheItem.Buffer;
            }

            // 2. Key缓存未命中，从公共池获取或创建新缓冲区
            byte[] newBuffer = null;
            // 尝试从公共池取可用缓冲区
            while (_byteBufferPool.TryTake(out newBuffer))
            {
                // 找到尺寸足够的缓冲区则停止
                if (newBuffer.Length >= totalSize)
                    break;
                // 尺寸不足则跳过，继续找下一个
                newBuffer = null;
            }

            // 3. 公共池也没有合适的，创建新数组
            if (newBuffer == null)
            {
                newBuffer = new byte[totalSize];
            }

            // 4. 更新Key缓存
            _keyedBufferCache[videoKey] = new FrameBufferCacheItem(newBuffer, width, height, totalSize);
            return newBuffer;
        }

        /// <summary>
        /// 归还RGB24缓冲区到缓存池
        /// </summary>
        /// <param name="videoKey">视频唯一标识</param>
        /// <param name="buffer">要归还的缓冲区</param>
        public static void ReturnRgb24Buffer(string videoKey, byte[] buffer)
        {
            if (string.IsNullOrEmpty(videoKey) || buffer == null)
                return;

            // 移除该Key对应的缓存项
            if (_keyedBufferCache.TryRemove(videoKey, out var cacheItem) && cacheItem.Buffer == buffer)
            {
                // 将缓冲区归还到公共池（不超过最大数量）
                if (_byteBufferPool.Count < MaxPoolCount)
                {
                    _byteBufferPool.Add(buffer);
                }
            }
        }

        /// <summary>
        /// 清理指定视频Key的所有缓存
        /// </summary>
        /// <param name="videoKey">视频唯一标识</param>
        public static void ClearCache(string videoKey)
        {
            if (string.IsNullOrEmpty(videoKey))
                return;

            if (_keyedBufferCache.TryRemove(videoKey, out var cacheItem))
            {
                // 清理数据并归还到公共池
                Array.Clear(cacheItem.Buffer, 0, cacheItem.TotalSize);
                if (_byteBufferPool.Count < MaxPoolCount)
                {
                    _byteBufferPool.Add(cacheItem.Buffer);
                }
            }
        }

        /// <summary>
        /// 清理所有缓存（程序退出/资源释放时调用）
        /// </summary>
        public static void ClearAllCache()
        {
            // 清理Key关联缓存
            foreach (var cacheItem in _keyedBufferCache.Values)
            {
                Array.Clear(cacheItem.Buffer, 0, cacheItem.TotalSize);
                if (_byteBufferPool.Count < MaxPoolCount)
                {
                    _byteBufferPool.Add(cacheItem.Buffer);
                }
            }
            _keyedBufferCache.Clear();

            // 清空公共池（可选，释放内存）
            while (_byteBufferPool.TryTake(out _)) { }
        }

        /// <summary>
        /// 获取当前缓存池状态（调试用）
        /// </summary>
        /// <returns>缓存池统计信息</returns>
        public static string GetPoolStatus()
        {
            return $"公共缓冲区池数量：{_byteBufferPool.Count}，Key关联缓存数量：{_keyedBufferCache.Count}";
        }

        #region 内部辅助类
        /// <summary>
        /// 帧缓冲区缓存项（存储尺寸和缓冲区关联）
        /// </summary>
        internal class FrameBufferCacheItem
        {
            public byte[] Buffer { get; }
            public int Width { get; }
            public int Height { get; }
            public int TotalSize { get; }

            public FrameBufferCacheItem(byte[] buffer, int width, int height, int totalSize)
            {
                Buffer = buffer ?? throw new ArgumentNullException(nameof(buffer));
                Width = width;
                Height = height;
                TotalSize = totalSize;
            }
        }
        #endregion
    }
}