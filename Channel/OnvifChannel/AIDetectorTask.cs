using ChannelUtility.Message;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using ZLMediaKit.Autogen;

namespace OnvifChannel
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
            _globalDefaultFont = GetFontByFamilyName("SimSun", 24) // 宋体（Windows）
                                    ?? GetFontByFamilyName("PingFang SC", 24) // 苹方（macOS）
                                    ?? GetFontByFamilyName("Noto Sans CJK SC", 24) // 思源黑体（Linux）
                                    ?? SystemFonts.Families.FirstOrDefault().CreateFont(24);
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
        /// <param name="listener"></param>
        /// <param name="data"></param>
        /// <param name="needDraw"></param>
        /// <param name="motion"></param>
        /// <param name="pool"></param>
        public static void Detect(VideoData videoData, int width, int height, OnvifDeviceEventListener listener, byte[] data, bool needDraw, MotionDetector motion, ByteArrayPool pool)
        {
            _ = Task.Run(() =>
            {
                try
                {
                    // 执行AI检测
                    var (isMotionDetected, motionRatio) = motion.IsMotionKeyframe(data, width, height);
                    if (isMotionDetected || videoData.NeedUp || needDraw)
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
                    }
                }
                finally
                {
                    pool.Return(data);
                }

            });

        }
        public static void Draw(IntPtr rgbFrame, IntPtr yuvLineSizes, int pixfmt, int width, int height, List<BoxItem> boxs)
        {
            var tmpboxArr = boxs;
            if (tmpboxArr.Count == 0)
            {
                return;
            }
            foreach (var box in tmpboxArr)
            {
                int x1 = (int)Math.Max(0, box.x1);
                int y1 = (int)Math.Max(0, box.y1);
                int x2 = (int)Math.Min(width - 1, box.x2);
                int y2 = (int)Math.Min(height - 1, box.y2);

                HexToRgb(box.color, out byte r, out byte g, out byte b);
                LibConvert.yuv_render(rgbFrame, yuvLineSizes, width, height, pixfmt, x1, y1, x2, y2, r, g, b, box.label);
            }
        }
        /// <summary>
        /// 将 #ff0000 格式颜色转为 byte r, byte g, byte b
        /// </summary>
        private static void HexToRgb(string hex, out byte r, out byte g, out byte b)
        {
            // 去掉 # 号
            hex = hex.TrimStart('#');

            // 转成整数
            int color = Convert.ToInt32(hex, 16);

            // 拆分 RGB
            r = (byte)((color >> 16) & 0xFF);
            g = (byte)((color >> 8) & 0xFF);
            b = (byte)(color & 0xFF);
        }
    }
}
