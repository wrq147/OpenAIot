using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixVideoChannel
{
    /// <summary>
    /// 画面变化检测
    /// </summary>
    public class MotionDetector
    {
        // 像素块运动差异阈值（块亮度差>此值判定为运动）
        public int BlockDiffThreshold { get; set; } = 30;
        // 运动块占比阈值（0-1，如0.08=8%块运动即触发）
        public float MotionBlockRatioThreshold { get; set; } = 0.08f;
        // 最小运动块数（过滤微小抖动）
        public int MinMotionBlocks { get; set; } = 20;
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
            public byte[] LastGrayPixels { get; set; }
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
        public bool IsMotionKeyframe(byte[] rgb24Data, int width, int height)
        {
            try
            {
                // 1. 冷却时间判断
                long currentTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                if (currentTime - _context.LastTriggerTime < CoolDownMs)
                {
                    return false;
                }

                // 2. 将RGB24字节数组转为灰度图
                using var image = Image.LoadPixelData<Rgb24>(rgb24Data, width, height);
                using var grayImage = image.Clone(x => x.Grayscale());

                // 3. 提取灰度像素数组
                byte[] currentGrayPixels = GetGrayPixelArray(grayImage);

                // 4. 第一帧初始化（无参考帧，直接返回false）
                if (_context.LastGrayPixels == null || _context.LastGrayPixels.Length != currentGrayPixels.Length)
                {
                    UpdateContext(currentGrayPixels, width, height);
                    return false;
                }

                // 5. 像素块运动检测（核心判断逻辑）
                bool isMotionDetected = CheckBlockMotion(
                    _context.LastGrayPixels, currentGrayPixels,
                    _context.LastWidth, _context.LastHeight);

                // 6. 更新上下文
                UpdateContext(currentGrayPixels, width, height);

                // 7. 标记触发时间
                if (isMotionDetected)
                {
                    _context.LastTriggerTime = currentTime;
                }

                return isMotionDetected;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"运动检测异常：{ex.Message}");
                return false;
            }
        }

        #region 核心工具方法（使用实例专属配置）
   

        /// <summary>
        /// 提取灰度图像的像素数组（单通道）
        /// </summary>
        private byte[] GetGrayPixelArray(Image<Rgb24> grayImage)
        {
            int width = grayImage.Width;
            int height = grayImage.Height;
            byte[] pixels = new byte[width * height];
            int index = 0;

            // ImageSharp的灰度图中，R=G=B，取R通道即可
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    pixels[index++] = grayImage[x, y].R;
                }
            }

            return pixels;
        }

        /// <summary>
        /// 像素块运动检测（统计运动块占比）
        /// </summary>
        private bool CheckBlockMotion(byte[] lastGray, byte[] currentGray, int width, int height)
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
                    int lastAvg = GetBlockAverageBrightness(lastGray, width, height, blockStartX, blockStartY);
                    int currentAvg = GetBlockAverageBrightness(currentGray, width, height, blockStartX, blockStartY);
                    int diff = Math.Abs(lastAvg - currentAvg);

                    // 亮度差超过实例专属阈值，判定为运动块
                    if (diff > BlockDiffThreshold)
                    {
                        motionBlockCount++;
                    }
                }
            }

            // 计算运动块占比
            int totalBlocks = blockCountX * blockCountY;
            float motionRatio = (float)motionBlockCount / totalBlocks;

            // 运动块占比超阈值 且 数量足够 → 判定为有运动
            return motionRatio > MotionBlockRatioThreshold && motionBlockCount > MinMotionBlocks;
        }

        /// <summary>
        /// 计算像素块的平均亮度
        /// </summary>
        private int GetBlockAverageBrightness(byte[] grayPixels, int width, int height, int startX, int startY)
        {
            long sum = 0;
            int pixelCount = 0;

            for (int y = startY; y < startY + BlockSize && y < height; y++)
            {
                for (int x = startX; x < startX + BlockSize && x < width; x++)
                {
                    int index = y * width + x;
                    sum += grayPixels[index];
                    pixelCount++;
                }
            }

            return pixelCount == 0 ? 0 : (int)(sum / pixelCount);
        }

        /// <summary>
        /// 更新实例上下文
        /// </summary>
        private void UpdateContext(byte[] grayPixels, int width, int height)
        {
            _context.LastGrayPixels = (byte[])grayPixels.Clone();
            _context.LastWidth = width;
            _context.LastHeight = height;
        }
        #endregion

        /// <summary>
        /// 重置当前实例的上下文（比如视频流重启时）
        /// </summary>
        public void ResetContext()
        {
            _context.LastGrayPixels = null;
            _context.LastTriggerTime = 0;
        }
    }

}
