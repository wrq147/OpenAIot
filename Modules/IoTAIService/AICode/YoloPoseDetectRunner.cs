using ChannelUtility.Message;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Options;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IoTAIService.AICode
{
    public class YoloPoseDetectRunner
    {
        public YoloPoseDetectRunner()
        {

        }

        public List<BoxItem> Predict(Image<Rgb24> image, float confidenceThreshold)
        {
            var inferenceSession = InferenceSessionPool.Instance.GetInferenceSession(nameof(YoloPoseDetectRunner), () =>
            {
                // 初始化ONNX推理会话
                var sessionOptions = new SessionOptions();
                var provider = AIUtility.TryEnableGpu(sessionOptions);
                string modelName = "YoloPose.onnx";
                if (provider != ExecutionProviderType.CPU)
                {
                    modelName = "YoloPoseS.onnx";
                }
                string modelPath = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + @"AIModel" + Path.DirectorySeparatorChar + modelName;
                return new InferenceSession(modelPath, sessionOptions);
            });
            try
            {
                int originalWidth = image.Width;
                int originalHeight = image.Height;
                float gain = Math.Min(640.0f / originalWidth, 640.0f / originalHeight);
                // 1. 图像预处理（与训练时保持一致）
                var inputTensor = PreprocessImage(image, gain);
                // 2. 准备输入
                var inputs = new List<NamedOnnxValue> {
                 NamedOnnxValue.CreateFromTensor("images", inputTensor),
            };

                // 3. 执行推理
                using var outputs = inferenceSession.Run(inputs);
                var outputTensor = outputs.First().AsTensor<float>();
                // 4. 后处理解析结果
                var detectionResults = PostprocessOutput(outputTensor, confidenceThreshold, originalWidth, originalHeight, gain);

                return detectionResults;
            }
            finally
            {
                InferenceSessionPool.Instance.ReleaseSession(nameof(YoloPoseDetectRunner), inferenceSession);
            }

        }
        /// <summary>
        /// 图像预处理：缩放、归一化等
        /// </summary>
        /// <param name="input"></param>
        /// <param name="gain"></param>
        /// <returns></returns>
        private Tensor<float> PreprocessImage(Image<Rgb24> input, float gain)
        {
            using Image<Rgb24> image = input.Clone();
            int originalWidth = image.Width;
            int originalHeight = image.Height;
            // 计算缩放比例（保持宽高比，填充黑边）
            float resizedWidth = originalWidth * gain;
            float resizedHeight = originalHeight * gain;
            float dw = (640 - resizedWidth) / 2.0f;
            float dh = (640 - resizedHeight) / 2.0f;
            if (originalWidth != resizedWidth || originalHeight != resizedHeight)
            {
                image.Mutate(x => x.Resize(
                    width: (int)resizedWidth,
                    height: (int)resizedHeight,
                    sampler: KnownResamplers.Triangle));
            }
            int top = (int)Math.Round(dh - 0.1f);
            int left = (int)Math.Round(dw - 0.1f);

            // 7. 填充黑边（对齐cv2.copyMakeBorder）
            using Image<Rgb24> paddedImg = new Image<Rgb24>(640, 640);
            paddedImg.Mutate(x =>
            {
                // 填充背景色
                x.Fill(new Rgb24(114, 114, 114));
                // 粘贴缩放后的图片（top/left为填充量）
                x.DrawImage(image, new Point(left, top), 1.0f);
            });

            // 转换为张量
            var tensor = new DenseTensor<float>(new[] { 1, 3, 640, 640 });
            for (int y = 0; y < 640; y++)
            {
                for (int x = 0; x < 640; x++)
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
        /// 后处理：解析模型输出，过滤低置信度结果，执行NMS，还原检测框到原图坐标
        /// </summary>
        /// <param name="output"></param>
        /// <param name="confidenceThreshold"></param>
        /// <param name="originalWidth"></param>
        /// <param name="originalHeight"></param>
        /// <param name="gain"></param>
        /// <returns></returns>
        private List<BoxItem> PostprocessOutput(Tensor<float> output, float confidenceThreshold, int originalWidth, int originalHeight, float gain)
        {
            var results = new List<BoxItem>();
            int maxBoxes = output.Dimensions[1];
            int numKeypoints = 17;
            float paddingX = (640 - originalWidth * gain) / 2;
            float paddingY = (640 - originalHeight * gain) / 2;
            for (int i = 0; i < maxBoxes; i++)
            {
                float conf = output[0, i, 4];

                // ============================
                // 置信度=0 或 低于阈值 = 填充框，直接跳过
                // ============================
                if (conf < confidenceThreshold || conf <= 0.001f)
                    continue;

                // 1. 读取框 (x1 y1 x2 y2)
                float x1 = output[0, i, 0];
                float y1 = output[0, i, 1];
                float x2 = output[0, i, 2];
                float y2 = output[0, i, 3];
                float cls = output[0, i, 5];

                // 2. 读取17个关键点
                List<KeyPoint> keypoints = new List<KeyPoint>();
                for (int k = 0; k < numKeypoints; k++)
                {
                    int idx = 6 + k * 3;
                    float kpx = output[0, i, idx];
                    float kpy = output[0, i, idx + 1];
                    float kpc = output[0, i, idx + 2];
                    keypoints.Add(new KeyPoint
                    {
                        x = kpx,
                        y = kpy,
                        score = kpc
                    });
                }

                // 3. 坐标还原（去padding + 缩放）
                x1 = (x1 - paddingX) / gain;
                y1 = (y1 - paddingY) / gain;
                x2 = (x2 - paddingX) / gain;
                y2 = (y2 - paddingY) / gain;

                x1 = Math.Clamp(x1, 0, originalWidth);
                y1 = Math.Clamp(y1, 0, originalHeight);
                x2 = Math.Clamp(x2, 0, originalWidth);
                y2 = Math.Clamp(y2, 0, originalHeight);

                for (int k = 0; k < keypoints.Count; k++)
                {
                    var kp = keypoints[k];
                    float rx = (kp.x - paddingX) / gain;
                    float ry = (kp.y - paddingY) / gain;
                    keypoints[k] = new KeyPoint
                    {
                        x = Math.Clamp(rx, 0, originalWidth),
                        y = Math.Clamp(ry, 0, originalHeight),
                        score = kp.score
                    };
                }

                // 4. 添加结果
                results.Add(new BoxItem
                {
                    label = "人",
                    score = conf,
                    x1 = (int)x1,
                    y1 = (int)y1,
                    x2 = (int)x2,
                    y2 = (int)y2,
                    color = "#097C28",
                    points = keypoints
                });
            }

            return results;
        }

    }
}
