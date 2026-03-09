using ChannelUtility.Message;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTAIService.AICode
{
    public static class AIUtility
    {
        #region 全局字体配置（静态字段）
        // 全局默认字体（静态只读，仅初始化一次）
        private static readonly Font _globalDefaultFont;
        private static readonly Brush _whiteBrush = new SolidBrush(Color.White);
        // 静态构造函数：初始化全局字体（仅在类第一次被使用时执行）
        static AIUtility()
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
        public static string DrawJpeg(Image<Rgb24> image, List<BoxItem> boxs)
        {
            var tmpboxArr = boxs;
            if (tmpboxArr.Count == 0)
            {
                return image.ToBase64String(JpegFormat.Instance);
            }

            // 遍历所有检测框
            foreach (var box in tmpboxArr)
            {
                // 1. 坐标校验与裁剪（防止越界）
                int x1 = (int)Math.Max(0, box.x1);
                int y1 = (int)Math.Max(0, box.y1);
                int x2 = (int)Math.Min(image.Width - 1, box.x2);
                int y2 = (int)Math.Min(image.Height - 1, box.y2);

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
            return image.ToBase64String(JpegFormat.Instance);
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
        public static ExecutionProviderType TryEnableGpu(SessionOptions sessionOptions)
        {
            // 1. 获取系统所有可用执行提供者
            var allAvailableProviders = OrtEnv.Instance().GetAvailableProviders();
            Console.WriteLine($"📜 系统检测到的所有执行提供者：{string.Join(", ", allAvailableProviders)}");

            // 2. 过滤掉 CPU 提供者，只保留 GPU 类（含国产）
            var cpuProviders = new List<string> { "CPUExecutionProvider" };
            var gpuProviders = allAvailableProviders.Where(p => !cpuProviders.Contains(p)).ToList();

            if (!gpuProviders.Any())
            {
                Console.WriteLine("⚠️ 未检测到任何 GPU 执行提供者，使用 CPU 推理");
                return ExecutionProviderType.CPU;
            }

            // 3. 定义执行提供者映射（含国产 GPU 关键词匹配）
            var providerMapping = new Dictionary<string, ExecutionProviderType>
            {
                // 传统 GPU
                { "TensorrtExecutionProvider", ExecutionProviderType.NVIDIA_TensorRT },
                { "CUDAExecutionProvider", ExecutionProviderType.NVIDIA_CUDA },
                { "DmlExecutionProvider", ExecutionProviderType.AMD_DirectML },
                // 国产 GPU（按厂商关键词匹配，需根据实际提供者名称调整）
                { "AscendExecutionProvider", ExecutionProviderType.Ascend_CANN },   // 华为昇腾
                { "DCUExecutionProvider", ExecutionProviderType.Hygon_DCU },         // 海光 DCU
                { "BirenExecutionProvider", ExecutionProviderType.Biren_BRPC },      // 壁仞
                { "CANNExecutionProvider", ExecutionProviderType.Ascend_CANN },      // 昇腾别名
                { "MLUExecutionProvider", ExecutionProviderType.Other_GPU },         // 寒武纪 MLU
                { "KunlunXinExecutionProvider", ExecutionProviderType.Other_GPU },   // 昆仑芯
                { "SambanovaExecutionProvider", ExecutionProviderType.Other_GPU }    // 天数智芯
            };

            // 4. 遍历所有 GPU 提供者，逐个尝试启用（按检测到的顺序）
            foreach (var providerName in gpuProviders)
            {
                try
                {
                    // 根据提供者名称动态配置（适配大部分 GPU 的通用接口）
                    var providerType = providerMapping.TryGetValue(providerName, out var type)
                        ? type : ExecutionProviderType.Other_GPU;

                    Console.WriteLine($"🔍 尝试启用 {providerType}（提供者名称：{providerName}）");

                    // 通用化启用逻辑（适配 90% 的 GPU 执行提供者，含国产）
                    EnableExecutionProvider(sessionOptions, providerName);

                    // 验证是否启用成功（创建临时会话测试）
                    using var tempSession = new InferenceSession("", sessionOptions);
                    Console.WriteLine($"✅ 成功启用 {providerType} GPU");
                    return providerType;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ 启用 {providerName} 失败：{ex.Message.Substring(0, Math.Min(100, ex.Message.Length))}，尝试下一个");
                    continue;
                }
            }

            // 所有 GPU 都启用失败，降级到 CPU
            Console.WriteLine("⚠️ 所有 GPU 执行提供者启用失败，使用 CPU 推理");
            return ExecutionProviderType.CPU;
        }
        private static void EnableExecutionProvider(SessionOptions sessionOptions, string providerName)
        {
            // 设备 ID 默认用 0
            int deviceId = 0;
            // 按提供者名称调用对应启用方法（适配 ONNX Runtime 标准接口）
            switch (providerName.ToLower())
            {
                case "cudaexecutionprovider":
                    sessionOptions.AppendExecutionProvider_CUDA(deviceId);
                    break;
                case "tensorrtexecutionprovider":
                    sessionOptions.AppendExecutionProvider_Tensorrt(deviceId);
                    break;
                case "dmlexecutionprovider":
                    sessionOptions.AppendExecutionProvider_DML(deviceId);
                    break;
                // 国产 GPU 通用启用方式（大部分厂商遵循 ONNX Runtime 扩展接口）
                case "ascendexecutionprovider":
                case "cannexecutionprovider":
                case "dcuexecutionprovider":
                case "birenexecutionprovider":
                default:
                    // 通用扩展接口：通过参数配置启用（适配国产 GPU）
                    var providerOptions = new Dictionary<string, string>
                    {
                        { "device_id", deviceId.ToString() }
                    };
                    sessionOptions.AppendExecutionProvider(providerName, providerOptions);
                    break;
            }
        }

        /// <summary>
        /// 将float[][]转换为指定形状的DenseTensor
        /// </summary>
        /// <param name="twoDArray">输入的二维浮点数组（如N个512维向量 → float[N][512]）</param>
        /// <param name="targetShape">目标张量形状（YOLO-World需为 new[] {1, N, 512}）</param>
        /// <returns>连续内存的DenseTensor</returns>
        /// <exception cref="ArgumentException">形状不匹配时抛出</exception>
        public static DenseTensor<float> ToDenseTensor(this float[][] twoDArray, int[] targetShape)
        {
            if (twoDArray == null || twoDArray.Length == 0)
                throw new ArgumentNullException(nameof(twoDArray), "二维数组不能为空");
            if (targetShape == null || targetShape.Length != 3)
                throw new ArgumentException("目标形状需为3维（batch, num_classes, embed_dim）", nameof(targetShape));

            // 1. 验证形状匹配：targetShape[1] = 类别数，targetShape[2] = 嵌入维度
            int numClasses = targetShape[1];
            int embedDim = targetShape[2];
            if (twoDArray.Length != numClasses)
                throw new ArgumentException($"二维数组行数({twoDArray.Length})需等于类别数({numClasses})");
            if (twoDArray.Any(row => row.Length != embedDim))
                throw new ArgumentException($"二维数组每行长度需等于嵌入维度({embedDim})");

            // 2. 展平float[][]为一维float[]（连续内存）
            float[] flatArray = new float[numClasses * embedDim];
            int index = 0;
            foreach (var row in twoDArray)
            {
                Array.Copy(row, 0, flatArray, index, embedDim);
                index += embedDim;
            }

            // 3. 创建DenseTensor<float>（核心：指定目标形状）
            return new DenseTensor<float>(flatArray, targetShape);
        }
    }
}
