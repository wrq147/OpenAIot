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
    public class OcrRecRunner
    {
        private readonly List<string> _vocab;
        public OcrRecRunner()
        {
            string dictPath = Path.Combine(Directory.GetCurrentDirectory(), "AIModel", "ppocr5_dict.txt");
            _vocab = File.ReadAllLines(dictPath, System.Text.Encoding.UTF8).ToList();
            _vocab.Add(" "); // 空格
        }

        public string Predict(Image<Rgb24> image)
        {
            var inferenceSession = InferenceSessionPool.Instance.GetInferenceSession(nameof(OcrRecRunner), () =>
            {
                // 初始化ONNX推理会话
                var sessionOptions = new SessionOptions();
                AIUtility.TryEnableGpu(sessionOptions);
                string modelName = "OcrRec.onnx";
                string modelPath = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + @"AIModel" + Path.DirectorySeparatorChar + modelName;
                return new InferenceSession(modelPath, sessionOptions);
            });
            try
            {
                // 1. 图像预处理（与训练时保持一致）
                var inputImage = TransposeVerticalImage(image);
                var inputTensor = PreprocessImage(inputImage);
                // 2. 准备输入
                var inputs = new List<NamedOnnxValue> {    
                    // 图片输入：假设已预处理为(1,3,48,320)的Tensor<float>
                    NamedOnnxValue.CreateFromTensor("x", inputTensor)
                };

                // 3. 执行推理
                using var outputs = inferenceSession.Run(inputs);
                var outputTensor = outputs.First().AsTensor<float>();
                return CtcDecode(outputTensor);
            }
            finally
            {
                InferenceSessionPool.Instance.ReleaseSession(nameof(OcrRecRunner), inferenceSession);
            }

        }
        /// <summary>
        /// 纵向图片像素转置：宽高互换，文字不旋转
        /// </summary>
        private Image<Rgb24> TransposeVerticalImage(Image<Rgb24> input)
        {
            // 横向图直接返回
            if (input.Height <= input.Width)
                return input;

            input.Mutate(x => x.Rotate(RotateMode.Rotate90));
            return input;
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
            float gain = Math.Min(320.0f / originalWidth, 48.0f / originalHeight);
            Image<Rgb24> image = input.Clone();
            // 计算缩放比例（保持宽高比，填充黑边）
            float resizedWidth = originalWidth * gain;
            float resizedHeight = originalHeight * gain;
            float dw = (320 - resizedWidth) / 2.0f;
            float dh = (48 - resizedHeight) / 2.0f;
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
            Image<Rgb24> paddedImg = new Image<Rgb24>(320, 48);
            paddedImg.Mutate(x =>
            {
                // 填充背景色
                x.Fill(new Rgb24(114, 114, 114));
                // 粘贴缩放后的图片（top/left为填充量）
                x.DrawImage(image, new Point(left, top), 1.0f);
            });

            // 转换为张量
            var tensor = new DenseTensor<float>(new[] { 1, 3, 48, 320 });
            for (int y = 0; y < 48; y++)
            {
                for (int x = 0; x < 320; x++)
                {
                    var pixel = paddedImg[x, y];
                    // 归一化
                    tensor[0, 0, y, x] = pixel.R / 255f;
                    tensor[0, 1, y, x] = pixel.G / 255f;
                    tensor[0, 2, y, x] = pixel.B / 255f;
                }
            }
            return tensor;
        }

        /// <summary>
        /// CTC 解码，去重+去除空白占位
        /// </summary>
        private string CtcDecode(Tensor<float> output)
        {
            int seqLen = output.Dimensions[1];
            int classNum = output.Dimensions[2];

            List<int> indexList = new List<int>();
            for (int t = 0; t < seqLen; t++)
            {
                int maxIdx = 0;
                float maxScore = float.MinValue;
                for (int c = 0; c < classNum; c++)
                {
                    float score = output[0, t, c];
                    if (score > maxScore)
                    {
                        maxScore = score;
                        maxIdx = c;
                    }
                }
                indexList.Add(maxIdx);
            }

            // CTC 规则：跳过0(空白)、连续重复只保留一个
            List<int> resultIdx = new List<int>();
            int lastIdx = -1;
            foreach (int idx in indexList)
            {
                if (idx != 0 && idx != lastIdx)
                {
                    resultIdx.Add(idx);
                    lastIdx = idx;
                }
            }

            // 映射为文字
            return string.Concat(resultIdx
                .Where(i => i < _vocab.Count)
                .Select(i => _vocab[i]));
        }
    }
}
