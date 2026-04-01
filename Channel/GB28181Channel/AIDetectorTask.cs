using ChannelUtility.Message;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace GB28181Channel
{
    public static class AIDetectorTask
    {
        #region 全局字体配置（静态字段）
        // 全局默认字体（静态只读，仅初始化一次）
        private static readonly Font _globalDefaultFont;
        private static readonly Brush _whiteBrush = new SolidBrush(Color.White);
        // 静态构造函数：初始化全局字体（仅在类第一次被使用时执行）
        static AIDetectorTask()
        {
            _globalDefaultFont = GetFontByFamilyName("SimSun", 12) // 宋体（Windows）
                                    ?? GetFontByFamilyName("PingFang SC", 12) // 苹方（macOS）
                                    ?? GetFontByFamilyName("Noto Sans CJK SC", 12) // 思源黑体（Linux）
                                    ?? SystemFonts.Families.FirstOrDefault().CreateFont(12);
        }

        /// <summary>
        /// 根据字体家族名称创建字体
        /// </summary>
        /// <param name="familyName">字体家族名称</param>
        /// <param name="size">字体大小</param>
        /// <returns>字体实例（不存在则返回null）</returns>
        private static Font GetFontByFamilyName(string familyName, float size)
        {
            try
            {
                if (SystemFonts.TryGet(familyName, out FontFamily family))
                {
                    return family.CreateFont(size);
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
        #endregion

        private static byte[] FastJpgCompress(byte[] rawData, int width, int height)
        {
            // 1. 入参校验（避免无效数据导致异常）
            if (rawData == null || rawData.Length == 0
                || width <= 0 || height <= 0
                || rawData.Length != width * height * 3)
            {
                return null;
            }

            try
            {
                // 2. 预分配内存流（减少扩容开销，预估JPG大小为原数据的1/10）
                using (var ms = new MemoryStream(rawData.Length / 10))
                {
                    // 3. 加载RGB24数据到Image对象（直接映射像素，无额外拷贝）
                    using (var image = Image.LoadPixelData<Rgb24>(rawData, width, height))
                    {
                        // 4. 配置JPG编码器（优先速度，适配高性能场景）
                        var jpgEncoder = new JpegEncoder
                        {
                            Quality = 85,
                        };

                        // 5. 编码为JPG并写入内存流
                        image.Save(ms, jpgEncoder);
                        return ms.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                // 捕获异常避免崩溃（可选：根据业务需求记录日志）
                Console.WriteLine($"JPG压缩失败：{ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// AI检测
        /// </summary>
        /// <param name="videoData"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="motionRatio"></param>
        /// <param name="listener"></param>
        /// <param name="data"></param>
        public static void Detect(VideoData videoData, int width, int height, float motionRatio, GB28181DeviceEventListener listener, byte[] data)
        {
            _ = Task.Run(() =>
            {
                if (listener == null)
                {
                    return;
                }
                byte[] pressData = FastJpgCompress(data, width, height);
                List<AIConfigData> configs = null;
                if (videoData.NeedUp)
                {
                    configs = videoData.Configs;
                    videoData.NeedUp = false;
                }
                listener.OnSendAIDetectRequest(videoData.Item.Id, videoData.Item.PushKey, motionRatio, pressData, width, height, configs, 1);
            });
        }
        public static void Draw(byte[] rgbFrame, int width, int height, List<BoxItem> boxs)
        {
            var tmpboxArr = boxs;
            if (tmpboxArr.Count == 0)
            {
                return;
            }
            using var image = Image.LoadPixelData<Rgb24>(rgbFrame, width, height);

            // 遍历所有检测框
            foreach (var box in tmpboxArr)
            {
                // 1. 坐标校验与裁剪（防止越界）
                int x1 = (int)Math.Max(0, box.x1);
                int y1 = (int)Math.Max(0, box.y1);
                int x2 = (int)Math.Min(width - 1, box.x2);
                int y2 = (int)Math.Min(height - 1, box.y2);

                // 跳过无效框
                if (x1 >= x2 || y1 >= y2)
                {
                    continue;
                }

                // 2. 获取当前框的颜色
                Color color = Color.Parse(box.color);
                Rgb24 boxColor = color.ToPixel<Rgb24>();

                // 3. 绘制矩形边框
                int lineWidth = 2;
                DrawRectangle(image, x1, y1, x2, y2, boxColor, lineWidth);

                // 4. 绘制标签背景和文字
                string labelText = $"{box.label} {box.score:F2}";
                DrawLabel(image, x1, y1, labelText, boxColor);
            }

            // 将绘制后的图像数据写回rgbFrame
            image.CopyPixelDataTo(rgbFrame);
        }
        /// <summary>
        /// 绘制矩形边框
        /// </summary>
        private static void DrawRectangle(Image<Rgb24> image, int x1, int y1, int x2, int y2, Rgb24 color, int lineWidth)
        {
            int width = image.Width;
            int height = image.Height;

            // 绘制上边框
            for (int y = y1; y < y1 + lineWidth && y < height; y++)
            {
                for (int x = x1; x <= x2 && x < width; x++)
                {
                    image[x, y] = color;
                }
            }

            // 绘制下边框
            for (int y = y2 - lineWidth + 1; y <= y2 && y < height; y++)
            {
                for (int x = x1; x <= x2 && x < width; x++)
                {
                    image[x, y] = color;
                }
            }

            // 绘制左边框
            for (int x = x1; x < x1 + lineWidth && x < width; x++)
            {
                for (int y = y1; y <= y2 && y < height; y++)
                {
                    image[x, y] = color;
                }
            }

            // 绘制右边框
            for (int x = x2 - lineWidth + 1; x <= x2 && x < width; x++)
            {
                for (int y = y1; y <= y2 && y < height; y++)
                {
                    image[x, y] = color;
                }
            }
        }

        /// <summary>
        /// 绘制标签（背景框+文字）
        /// </summary>
        private static void DrawLabel(Image<Rgb24> image, int x, int y, string text, Rgb24 color)
        {
            if (string.IsNullOrEmpty(text)) return;

            int width = image.Width;
            int height = image.Height;

            // 计算文字尺寸（使用全局默认字体）
            var textOptions = new TextOptions(_globalDefaultFont);
            var textSize = TextMeasurer.MeasureSize(text, textOptions);

            // 标签内边距（优化视觉效果）
            int paddingX = 4;
            int paddingY = 2;
            int labelWidth = (int)Math.Ceiling(textSize.Width) + 2 * paddingX;
            int labelHeight = (int)Math.Ceiling(textSize.Height) + 2 * paddingY;

            // 标签坐标（防止越界，向上偏移避免遮挡检测框）
            int labelX = Math.Max(0, x);
            int labelY = Math.Max(0, y - labelHeight);
            int labelX2 = Math.Min(width - 1, labelX + labelWidth);
            int labelY2 = Math.Min(height - 1, labelY + labelHeight);

            // 绘制标签背景（半透明）
            var backgroundBrush = new SolidBrush(Color.FromRgb(color.R, color.G, color.B).WithAlpha(128)); // 50%透明度
            image.Mutate(ctx => ctx.Fill(backgroundBrush, new RectangleF(labelX, labelY, labelX2 - labelX, labelY2 - labelY)));

            // 绘制文字（使用全局白色画刷，替代Brushes.White）
            image.Mutate(ctx => ctx.DrawText(text, _globalDefaultFont, _whiteBrush, new PointF(labelX + paddingX, labelY + paddingY)));
        }
    }
}
