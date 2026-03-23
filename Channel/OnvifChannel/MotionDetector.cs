using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnvifChannel
{
    /// <summary>
    /// 画面变化检测
    /// </summary>
    public class MotionDetector
    {
        // 建议值：10-80（越小越灵敏，10能检测极微小变化）
        public int RgbDiffThreshold { get; set; } = 10;
        // 运动块占比阈值（0-1，如0.2%块运动即触发）
        public float MotionBlockRatioThreshold { get; set; } = 0.001f;
        // 最小运动块数（过滤微小抖动）
        public int MinMotionBlocks { get; set; } = 10;
        // 像素块大小（8x8/16x16，越小越灵敏但计算量越大）
        public int BlockSize { get; set; } = 16;
        // 冷却时间（ms）：避免高频触发
        public int CoolDownMs { get; set; } = 200;

        // 实例上下文
        private readonly MotionContext _context = new MotionContext();
        /// <summary>
        /// 运动检测上下文
        /// </summary>
        private class MotionContext
        {
            // 上一帧灰度图像数据
            public byte[] LastPixels { get; set; }
            // 上一帧宽高
            public int LastWidth { get; set; }
            public int LastHeight { get; set; }
            // 上次触发时间
            public long LastTriggerTime { get; set; } = 0;
        }

        public MotionDetector()
        {
        }

        /// <summary>
        /// 判断当前BGR24帧是否为运动关键帧
        /// </summary>
        /// <param name="rgb24Data">RGB24格式帧数据</param>
        /// <param name="width">帧宽度</param>
        /// <param name="height">帧高度</param>
        /// <returns>是否需要执行AI检测</returns>
        public (bool, float) IsMotionKeyframe(byte[] rgb24Data, int width, int height)
        {
            try
            {
                // 1. 冷却时间判断
                long currentTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                if (currentTime - _context.LastTriggerTime < CoolDownMs)
                {
                    return (false, 0);
                }

                // 4. 第一帧初始化（无参考帧，直接返回false）
                if (_context.LastPixels == null || _context.LastPixels.Length != rgb24Data.Length)
                {
                    UpdateContext(rgb24Data, width, height);
                    return (false, 0);
                }

                // 5. 像素块运动检测
                var (isMotionDetected, motionRatio) = CheckBlockMotion(_context.LastPixels, rgb24Data, _context.LastWidth, _context.LastHeight);

                // 6. 更新上下文
                UpdateContext(rgb24Data, width, height);

                // 7. 标记触发时间
                if (isMotionDetected)
                {
                    _context.LastTriggerTime = currentTime;
                }

                return (isMotionDetected, motionRatio);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"运动检测异常：{ex.Message}");
                return (false, 0);
            }
        }

        #region 核心工具方法（使用实例专属配置）


        /// <summary>
        /// 像素块运动检测
        /// </summary>
        private (bool, float) CheckBlockMotion(byte[] lastRgb, byte[] currentRgb, int width, int height)
        {
            int blockCountX = width / BlockSize;
            int blockCountY = height / BlockSize;
            int motionBlockCount = 0;

            // 遍历所有像素块
            for (int by = 0; by < blockCountY; by++)
            {
                for (int bx = 0; bx < blockCountX; bx++)
                {
                    // 计算块的起始索引
                    int blockStartX = bx * BlockSize;
                    int blockStartY = by * BlockSize;

                    // 计算当前块和上一帧块的平均亮度差
                    int rgbDiff = CalculateRgbBlockDiff(lastRgb, currentRgb, width, height, blockStartX, blockStartY);

                    // RGB差异超过阈值，判定为运动块
                    if (rgbDiff > RgbDiffThreshold)
                    {
                        motionBlockCount++;
                    }
                }
            }

            // 计算运动块占比
            int totalBlocks = blockCountX * blockCountY;
            float motionRatio = (float)motionBlockCount / totalBlocks;

            // 运动块占比超阈值 且 数量足够 → 判定为有运动
            return (motionRatio > MotionBlockRatioThreshold && motionBlockCount > MinMotionBlocks, motionRatio);
        }

        /// <summary>
        /// 计算像素块的平均亮度
        /// </summary>
        private int CalculateRgbBlockDiff(byte[] lastRgb, byte[] currentRgb, int width, int height, int startX, int startY)
        {
            int totalDiff = 0;
            int pixelCount = 0;
            int pixelStride = 3;
            for (int y = startY; y < startY + BlockSize && y < height; y++)
            {
                for (int x = startX; x < startX + BlockSize && x < width; x++)
                {
                    int pixelIndex = (y * width + x) * pixelStride;
                    // 边界检查
                    if (pixelIndex + 2 >= lastRgb.Length || pixelIndex + 2 >= currentRgb.Length)
                        continue;

                    // 计算R/G/B三个通道的差值绝对值
                    int rDiff = Math.Abs(lastRgb[pixelIndex] - currentRgb[pixelIndex]);
                    int gDiff = Math.Abs(lastRgb[pixelIndex + 1] - currentRgb[pixelIndex + 1]);
                    int bDiff = Math.Abs(lastRgb[pixelIndex + 2] - currentRgb[pixelIndex + 2]);

                    // 累加为该像素的综合差异
                    totalDiff += rDiff + gDiff + bDiff;
                    ++pixelCount;
                }
            }

            return pixelCount == 0 ? 0 : totalDiff / pixelCount;
        }

        /// <summary>
        /// 更新实例上下文
        /// </summary>
        private void UpdateContext(byte[] pixels, int width, int height)
        {
            _context.LastPixels = new byte[pixels.Length];
            Array.Copy(pixels, _context.LastPixels, pixels.Length);
            _context.LastWidth = width;
            _context.LastHeight = height;
        }
        #endregion

        /// <summary>
        /// 重置当前实例的上下文（比如视频流重启时）
        /// </summary>
        public void ResetContext()
        {
            _context.LastPixels = null;
            _context.LastTriggerTime = 0;
        }
    }
}
