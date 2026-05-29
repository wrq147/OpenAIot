using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
namespace IoTAIService.AICode
{
    /// <summary>
    /// 文档图像方向分类
    /// </summary>
    public class OcrDocOriRunner
    {
        private readonly int[] _angles = { 0, 90, 180, 270 };
        public Image<Rgb24> AutoRotate(Image<Rgb24> image)
        {
            var inferenceSession = InferenceSessionPool.Instance.GetInferenceSession(nameof(OcrDocOriRunner), () =>
            {
                // 初始化ONNX推理会话
                var sessionOptions = new SessionOptions();
                AIUtility.TryEnableGpu(sessionOptions);
                string modelName = "OcrDocOri.onnx";
                string modelPath = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + @"AIModel" + Path.DirectorySeparatorChar + modelName;
                return new InferenceSession(modelPath, sessionOptions);
            });
            try
            {
                // 1. 图像预处理（与训练时保持一致）
                var inputTensor = PreprocessImage(image);
                // 2. 准备输入
                var inputs = new List<NamedOnnxValue> {    
                    // 图片输入：假设已预处理为(1,3,224,224)的Tensor<float>
                    NamedOnnxValue.CreateFromTensor("x", inputTensor)
                };

                // 3. 执行推理
                using var outputs = inferenceSession.Run(inputs);
                var outputTensor = outputs.First().AsTensor<float>();

                // 取最大置信度索引
                float maxScore = float.MinValue;
                int maxIndex = 0;
                for (int i = 0; i < 4; i++)
                {
                    if (outputTensor[0, i] > maxScore)
                    {
                        maxScore = outputTensor[0, i];
                        maxIndex = i;
                    }
                }
                int angle = _angles[maxIndex];

                if (angle == 90)
                    image.Mutate(x => x.Rotate(RotateMode.Rotate90));
                else if (angle == 180)
                    image.Mutate(x => x.Rotate(RotateMode.Rotate180));
                else if (angle == 270)
                    image.Mutate(x => x.Rotate(RotateMode.Rotate270));

                return image;
            }
            finally
            {
                InferenceSessionPool.Instance.ReleaseSession(nameof(OcrDocOriRunner), inferenceSession);
            }

        }

        /// <summary>
        /// 图像预处理：缩放、归一化等
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private DenseTensor<float> PreprocessImage(Image<Rgb24> input)
        {
            int originalWidth = input.Width;
            int originalHeight = input.Height;
            float gain = Math.Min(224.0f / originalWidth, 224.0f / originalHeight);
            Image<Rgb24> image = input.Clone();
            // 计算缩放比例（保持宽高比，填充黑边）
            float resizedWidth = originalWidth * gain;
            float resizedHeight = originalHeight * gain;
            float dw = (224 - resizedWidth) / 2.0f;
            float dh = (224 - resizedHeight) / 2.0f;
            if (originalWidth != resizedWidth || originalHeight != resizedHeight)
            {
                image.Mutate(x => x.Resize(
                    width: (int)resizedWidth,
                    height: (int)resizedHeight,
                    sampler: KnownResamplers.Triangle));
            }
            int top = (int)Math.Round(dh - 0.1f);
            int left = (int)Math.Round(dw - 0.1f);

            // 7. 填充黑边
            Image<Rgb24> paddedImg = new Image<Rgb24>(224, 224);
            paddedImg.Mutate(x =>
            {
                // 填充背景色
                x.Fill(new Rgb24(0, 0, 0));
                // 粘贴缩放后的图片（top/left为填充量）
                x.DrawImage(image, new Point(left, top), 1.0f);
            });

            // 转换为张量
            var tensor = new DenseTensor<float>(new[] { 1, 3, 224, 224 });
            for (int y = 0; y < 224; y++)
            {
                for (int x = 0; x < 224; x++)
                {
                    var pixel = paddedImg[x, y];
                    // 归一化
                    tensor[0, 0, y, x] = (pixel.R / 255f - 0.485f) / 0.229f; // R
                    tensor[0, 1, y, x] = (pixel.G / 255f - 0.456f) / 0.224f; // G
                    tensor[0, 2, y, x] = (pixel.B / 255f - 0.406f) / 0.225f; // B
                }
            }
            return tensor;
        }
    }
}
