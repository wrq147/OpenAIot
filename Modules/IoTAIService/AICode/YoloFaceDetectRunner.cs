using ChannelUtility.Message;
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

namespace IoTAIService.AICode
{
    public class YoloFaceDetectRunner
    {
        public YoloFaceDetectRunner()
        {

        }
        public List<BoxItem> Predict(Image<Rgb24> image, float confidenceThreshold = 0.7f, float iouThreshold = 0.45f)
        {
            var inferenceSession = InferenceSessionPool.Instance.GetInferenceSession(nameof(YoloFaceDetectRunner), () =>
            {
                var sessionOptions = new SessionOptions();
                var provider = AIUtility.TryEnableGpu(sessionOptions);
                string modelName = "YoloFace.onnx";
                if (provider != ExecutionProviderType.CPU)
                {
                    modelName = "YoloFaceS.onnx";
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
                // 图片输入：假设已预处理为(1,3,640,640)的Tensor<float>
                 NamedOnnxValue.CreateFromTensor("images", inputTensor),
            };

                // 3. 执行推理
                using var outputs = inferenceSession.Run(inputs);
                var outputTensor = outputs.First().AsTensor<float>();
                // 4. 后处理解析结果
                var detectionResults = PostprocessOutput(outputTensor, confidenceThreshold, iouThreshold, originalWidth, originalHeight, gain);

                return detectionResults;
            }
            finally
            {
                InferenceSessionPool.Instance.ReleaseSession(nameof(YoloFaceDetectRunner), inferenceSession);
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
            Image<Rgb24> image = input.Clone();
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
            Image<Rgb24> paddedImg = new Image<Rgb24>(640, 640);
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
        /// <param name="iouThreshold"></param>
        /// <param name="originalWidth"></param>
        /// <param name="originalHeight"></param>
        /// <param name="gain"></param>
        /// <returns></returns>
        private List<BoxItem> PostprocessOutput(Tensor<float> output, float confidenceThreshold, float iouThreshold, int originalWidth, int originalHeight, float gain)
        {
            var results = new List<BoxItem>();
            int numBoxes = output.Dimensions[2]; // 输出维度：[1, 4 + num_classes, num_boxes]

            // 计算padding
            float paddingX = (640 - (originalWidth * gain)) / 2 - 0.1f;
            float paddingY = (640 - (originalHeight * gain)) / 2 - 0.1f;
            // 解析每个检测框
            for (int i = 0; i < numBoxes; i++)
            {
                // 检测框坐标
                float cx = output[0, 0, i];
                float cy = output[0, 1, i];
                float w = output[0, 2, i];
                float h = output[0, 3, i];

                // 纯格式转换（严格对齐官方xywh2xyxy逐行逻辑）
                float wh_half_w = w / 2;
                float wh_half_h = h / 2;
                float x1 = cx - wh_half_w;
                float y1 = cy - wh_half_h;
                float x2 = cx + wh_half_w;
                float y2 = cy + wh_half_h;

                // 遍历所有类别，获取最高置信度的类别
                float maxConf = 0;
                int maxClassIdx = -1;
                for (int c = 0; c < 1; c++)
                {
                    float conf = output[0, 4 + c, i];
                    if (conf > maxConf)
                    {
                        maxConf = conf;
                        maxClassIdx = c;
                    }
                }

                // 过滤低置信度结果
                if (maxConf < confidenceThreshold || maxClassIdx == -1)
                    continue;


                // 直接去padding + 缩放到原图（无需×640，因为已是像素值）
                x1 = (x1 - paddingX) / gain;
                y1 = (y1 - paddingY) / gain;
                x2 = (x2 - paddingX) / gain;
                y2 = (y2 - paddingY) / gain;

                // 坐标范围限制（对齐官方clip_boxes）
                x1 = Math.Clamp(x1, 0, originalWidth);
                y1 = Math.Clamp(y1, 0, originalHeight);
                x2 = Math.Clamp(x2, 0, originalWidth);
                y2 = Math.Clamp(y2, 0, originalHeight);

                results.Add(new BoxItem
                {
                    label = "人脸",
                    score = maxConf,
                    x1 = (int)x1,
                    y1 = (int)y1,
                    x2 = (int)x2,
                    y2 = (int)y2,
                    color = "#67C23A"
                });
            }

            // 执行非极大值抑制（NMS），去除重叠框
            return ApplyNMS(results, iouThreshold);
        }

        /// <summary>
        /// 非极大值抑制（NMS）：去除重叠的高置信度框
        /// </summary>
        /// <param name="detections">检测结果列表</param>
        /// <param name="iouThreshold">IOU阈值</param>
        /// <returns>去重后的检测结果</returns>
        private List<BoxItem> ApplyNMS(List<BoxItem> detections, float iouThreshold)
        {
            var finalDetections = new List<BoxItem>();
            // 按类别分组处理
            foreach (var group in detections.GroupBy(d => d.label))
            {
                // 按置信度降序排序
                var sortedDetections = group.OrderByDescending(d => d.score).ToList();
                while (sortedDetections.Count > 0)
                {
                    var best = sortedDetections[0];
                    finalDetections.Add(best);
                    sortedDetections.RemoveAt(0);

                    // 计算当前框与剩余框的IOU，移除IOU超过阈值的框
                    sortedDetections = sortedDetections.Where(d =>
                    {
                        float iou = CalculateIOU(best, d);
                        return iou < iouThreshold;
                    }).ToList();
                }
            }
            return finalDetections;
        }

        /// <summary>
        /// 计算两个检测框的IOU（交并比）
        /// </summary>
        private float CalculateIOU(BoxItem a, BoxItem b)
        {
            float intersectX1 = Math.Max(a.x1, b.x1);
            float intersectY1 = Math.Max(a.y1, b.y1);
            float intersectX2 = Math.Min(a.x2, b.x2);
            float intersectY2 = Math.Min(a.y2, b.y2);

            if (intersectX1 >= intersectX2 || intersectY1 >= intersectY2)
                return 0;

            float intersectArea = (intersectX2 - intersectX1) * (intersectY2 - intersectY1);
            float areaA = (a.x2 - a.x1) * (a.y2 - a.y1);
            float areaB = (b.x2 - b.x1) * (b.y2 - b.y1);

            return intersectArea / (areaA + areaB - intersectArea);
        }
    }
}
