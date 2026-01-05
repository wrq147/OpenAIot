using ChannelUtility;
using ChannelUtility.Message;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO.Compression;
namespace FixVideoChannel
{
    public class AIDetectorTask
    {
        #region 全局字体配置（静态字段）
        // 全局默认字体（静态只读，仅初始化一次）
        private static readonly Font _globalDefaultFont;
        private static readonly Brush _whiteBrush = new SolidBrush(Color.White);
        // 静态构造函数：初始化全局字体（仅在类第一次被使用时执行）
        static AIDetectorTask()
        {
            _globalDefaultFont = GetFontByFamilyName("Arial", 12)
                                 ?? GetFontByFamilyName("SimSun", 12) // 宋体（Windows）
                                 ?? GetFontByFamilyName("PingFang SC", 12) // 苹方（macOS）
                                 ?? GetFontByFamilyName("Noto Sans", 12) // 思源黑体（Linux）
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
        private int _currentFrame;
        private AIDetectItem _item;
        private volatile List<BoxItem> _boxs;
        public AIDetectorTask(AIDetectItem item)
        {
            _item = item;
            _currentFrame = 0;
        }
        public void UpdateBoxList(string detType, List<BoxItem> items)
        {
            if (_item.Code == detType)
            {
                _boxs = items;
            }
        }
        /// <summary>
        /// Zlib快速压缩降采样后的BGR数据
        /// </summary>
        private byte[] FastZlibCompress(byte[] rawData, int width, int height, int scale = 2)
        {
            if (rawData == null || rawData.Length == 0)
                return null;

            // 第一步：先降采样
            byte[] downsampled = UltraFastDownsample(rawData, width, height, scale);

            // 第二步：Zlib快速压缩
            using (var ms = new MemoryStream())
            {
                using (var zlib = new DeflateStream(ms, CompressionLevel.Fastest, true))
                {
                    zlib.Write(downsampled, 0, downsampled.Length);
                }
                return ms.ToArray();
            }
        }

        // 复用之前的超极速降采样方法
        private byte[] UltraFastDownsample(byte[] rawData, int width, int height, int scale)
        {
            if (rawData == null || rawData.Length == 0 || scale <= 1)
                return rawData;

            int newWidth = width / scale;
            int newHeight = height / scale;
            int pixelSize = 3;
            byte[] result = new byte[newWidth * newHeight * pixelSize];

            int destIndex = 0;
            for (int y = 0; y < height; y += scale)
            {
                for (int x = 0; x < width; x += scale)
                {
                    int srcIndex = (y * width + x) * pixelSize;
                    if (srcIndex + 2 >= rawData.Length) break;

                    result[destIndex++] = rawData[srcIndex];
                    result[destIndex++] = rawData[srcIndex + 1];
                    result[destIndex++] = rawData[srcIndex + 2];
                }
            }
            return result;
        }

        // AI检测
        public void Detect(string videoId, int width, int height, IVideoDeviceEventListener listener, ref byte[] data, ref bool isPress)
        {
            _currentFrame++;
            if (listener == null)
            {
                return;
            }
            if (_currentFrame > _item.FraInter)
            {
                byte[] pressData = null;
                if (!isPress)
                {
                    data = FastZlibCompress(data, width, height, 2);
                    isPress = true;
                }

                pressData = data;
                Task t = listener.OnSendAIDetectRequest(videoId, _item, pressData, width / 2, height / 2);
                _currentFrame = 0;
            }
        }
        public bool Draw(byte[] bgrFrame, int width, int height)
        {
            if (!_item.EnableDraw)
            {
                return false;
            }
            var tmpboxArr = _boxs;
            if (tmpboxArr.Count == 0)
            {
                return false;
            }
            using var image = Image.LoadPixelData<Bgr24>(bgrFrame, width, height);

            // 遍历所有检测框
            foreach (var box in tmpboxArr)
            {
                // 1. 坐标校验与裁剪（防止越界）
                int x1 = (int)Math.Max(0, box.x1 * 2);
                int y1 = (int)Math.Max(0, box.y1 * 2);
                int x2 = (int)Math.Min(width - 1, box.x2 * 2);
                int y2 = (int)Math.Min(height - 1, box.y2 * 2);

                // 跳过无效框
                if (x1 >= x2 || y1 >= y2)
                {
                    continue;
                }

                // 2. 获取当前框的颜色
                Color color = Color.Parse(box.color);
                Bgr24 boxColor = color.ToPixel<Bgr24>();

                // 3. 绘制矩形边框
                int lineWidth = 2;
                DrawRectangle(image, x1, y1, x2, y2, boxColor, lineWidth);

                // 4. 绘制标签背景和文字
                string labelText = $"{box.label} {box.score:F2}";
                DrawLabel(image, x1, y1, labelText, boxColor);
            }

            // 将绘制后的图像数据写回bgrFrame
            image.CopyPixelDataTo(bgrFrame);
            return true;
        }
        /// <summary>
        /// 绘制矩形边框
        /// </summary>
        private void DrawRectangle(Image<Bgr24> image, int x1, int y1, int x2, int y2, Bgr24 color, int lineWidth)
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
        private void DrawLabel(Image<Bgr24> image, int x, int y, string text, Bgr24 color)
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
