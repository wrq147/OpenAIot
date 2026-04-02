using ChannelUtility.Message;
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
    public class MobileCLIP2VisionRunner : IReIDExtractor
    {
        private readonly InferenceSession _session;
        private const int InputWidth = 224;
        private const int InputHeight = 224;
        public MobileCLIP2VisionRunner()
        {
            string modelPath = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + @"AIModel" + Path.DirectorySeparatorChar + "MobileCLIP2Vision.onnx";
            var sessionOptions = new SessionOptions();
            AIUtility.TryEnableGpu(sessionOptions);
            _session = new InferenceSession(modelPath, sessionOptions);
        }
        public float[] ExtractFeature(Image<Rgb24> image, BoxItem roi)
        {
            var tmpimg = image.CropByBox(roi.x1, roi.x2, roi.y1, roi.y2);
            return OutputEmbeddings(tmpimg);
        }
        /// <summary>
        /// 输出图片特征
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public float[] OutputEmbeddings(Image<Rgb24> image)
        {
            image.Mutate(x => x.Resize(InputWidth, InputHeight));
            // 创建张量 [1, 3, 224, 224]
            var tensor = new DenseTensor<float>(new[] { 1, 3, InputHeight, InputWidth });

            for (int y = 0; y < InputHeight; y++)
            {
                for (int x = 0; x < InputWidth; x++)
                {
                    Rgb24 pixel = image[x, y];

                    tensor[0, 0, y, x] = pixel.R / 255f;
                    tensor[0, 1, y, x] = pixel.G / 255f;
                    tensor[0, 2, y, x] = pixel.B / 255f;
                }
            }

            // ====================== 2. ONNX 推理 ======================
            var inputs = new[] { NamedOnnxValue.CreateFromTensor("img_tensor", tensor) };
            using var results = _session.Run(inputs);
            float[] embeddings = results.First().AsTensor<float>().ToArray();

            // ====================== 3. 官方必须：L2 归一化 ======================
            return L2Normalize(embeddings);
        }
        /// <summary>
        /// L2归一化（MobileCLIP2 强制要求，否则余弦相似度无效）
        /// </summary>
        private float[] L2Normalize(float[] features)
        {
            float sumSq = 0;
            for (int i = 0; i < features.Length; i++)
                sumSq += features[i] * features[i];

            float norm = (float)Math.Sqrt(sumSq);
            if (norm < 1e-6f) norm = 1e-6f;

            float[] normalized = new float[features.Length];
            for (int i = 0; i < features.Length; i++)
                normalized[i] = features[i] / norm;

            return normalized;
        }

        /// <summary>
        /// 计算两个特征的余弦相似度
        /// </summary>
        public float CosineSimilarity(float[] feat1, float[] feat2)
        {
            float dot = 0;
            float mag1 = 0, mag2 = 0;
            for (int i = 0; i < feat1.Length; i++)
            {
                dot += feat1[i] * feat2[i];
                mag1 += feat1[i] * feat1[i];
                mag2 += feat2[i] * feat2[i];
            }
            return dot / ((float)Math.Sqrt(mag1) * (float)Math.Sqrt(mag2));
        }
    }
}
