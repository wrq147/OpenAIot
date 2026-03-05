using Microsoft.Extensions.Options;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace IoTAIService.AICode
{
    public class FaceSTNRunner
    {
        private readonly InferenceSession _session;

        public FaceSTNRunner()
        {
            string modelPath = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + @"AIModel" + Path.DirectorySeparatorChar + "FaceSTN.onnx";
            var sessionOptions = new SessionOptions();
            AIUtility.TryEnableGpu(sessionOptions);
            _session = new InferenceSession(modelPath, sessionOptions);
        }
        public Tensor<float> Predict(Image<Rgb24> image)
        {
            // 1. 图像预处理（与训练时保持一致）
            var inputTensor = PreprocessImage(image);

            // 2. 准备输入
            var inputs = new List<NamedOnnxValue> { NamedOnnxValue.CreateFromTensor("input", inputTensor) };

            // 3. 执行推理
            using var outputs = _session.Run(inputs);

            // 4. 解析输出
            var txoutputs = outputs.First(o => o.Name == "output").AsTensor<float>();

            return txoutputs;
        }
        // 图像预处理：缩放、归一化等
        private Tensor<float> PreprocessImage(Image<Rgb24> input)
        {
            // 缩放为128x128
            Image<Rgb24> image = input.Clone();
            image.Mutate(x => x.Resize(128, 128));

            // 转换为张量（NCHW格式：batch=1, channel=3, height=128, width=128）
            var tensor = new DenseTensor<float>(new[] { 1, 3, 128, 128 });
            for (int y = 0; y < 128; y++)
            {
                for (int x = 0; x < 128; x++)
                {
                    var pixel = image[x, y];
                    // 归一化（示例：ImageNet均值和标准差）
                    tensor[0, 0, y, x] = (pixel.R / 255f - 0.485f) / 0.229f;
                    tensor[0, 1, y, x] = (pixel.G / 255f - 0.456f) / 0.224f;
                    tensor[0, 2, y, x] = (pixel.B / 255f - 0.406f) / 0.225f;
                }
            }
            return tensor;
        }
    }
}
