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
    public class YoloWorldDetectRunner
    {
        private readonly InferenceSession _session;
        public class DetectionResult
        {
            public string Label { get; set; }    // 类别标签
            public float Confidence { get; set; } // 置信度
            public float X1 { get; set; }        // 检测框左上角X（原图坐标）
            public float Y1 { get; set; }        // 检测框左上角Y（原图坐标）
            public float X2 { get; set; }        // 检测框右下角X（原图坐标）
            public float Y2 { get; set; }        // 检测框右下角Y（原图坐标）
        }

        public YoloWorldDetectRunner()
        {
            string modelPath = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + @"AIModel" + Path.DirectorySeparatorChar + "YoloWorld.onnx";
            // 初始化ONNX推理会话
            var sessionOptions = new SessionOptions();
            AIUtility.TryEnableGpu(sessionOptions);
            _session = new InferenceSession(modelPath, sessionOptions);
        }

        public List<DetectionResult> Predict(Image<Rgb24> image, float confidenceThreshold, float iouThreshold, DenseTensor<float> classEmbeds, List<string> classes)
        {
            int originalWidth = image.Width;
            int originalHeight = image.Height;
            // 1. 图像预处理（与训练时保持一致）
            var inputTensor = PreprocessImage(image, out float scaleX, out float scaleY);
            // 2. 准备输入
            var inputs = new List<NamedOnnxValue> {    
                // 图片输入：假设已预处理为(1,3,640,640)的Tensor<float>
                 NamedOnnxValue.CreateFromTensor("images", inputTensor),
                // 文本嵌入输入：传入转换后的Tensor<float>
                NamedOnnxValue.CreateFromTensor("text_embeds", classEmbeds)
            };

            // 3. 执行推理
            using var outputs = _session.Run(inputs);
            var outputTensor = outputs.First().AsTensor<float>();
            // 4. 后处理解析结果
            var detectionResults = PostprocessOutput(outputTensor, confidenceThreshold, iouThreshold, scaleX, scaleY, originalWidth, originalHeight, classes);

            return detectionResults;
        }
        // 图像预处理：缩放、归一化等
        private Tensor<float> PreprocessImage(Image<Rgb24> input, out float scaleX, out float scaleY)
        {
            Image<Rgb24> image = input.Clone();
            int originalWidth = image.Width;
            int originalHeight = image.Height;
            // 计算缩放比例（保持宽高比，填充黑边）
            float ratio = Math.Min((float)640 / originalWidth, (float)640 / originalHeight);
            int resizedWidth = (int)(originalWidth * ratio);
            int resizedHeight = (int)(originalHeight * ratio);
            scaleX = (float)originalWidth / resizedWidth;
            scaleY = (float)originalHeight / resizedHeight;
            // 缩放为640x640
            image.Mutate(x => x.Resize(640, 640));

            // 转换为张量
            var tensor = new DenseTensor<float>(new[] { 1, 3, 640, 640 });
            for (int y = 0; y < 640; y++)
            {
                for (int x = 0; x < 640; x++)
                {
                    var pixel = image[x, y];
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
        /// <param name="scaleX"></param>
        /// <param name="scaleY"></param>
        /// <param name="originalWidth"></param>
        /// <param name="originalHeight"></param>
        /// <param name="classes"></param>
        /// <returns></returns>
        private List<DetectionResult> PostprocessOutput(Tensor<float> output, float confidenceThreshold, float iouThreshold, float scaleX, float scaleY, int originalWidth, int originalHeight, List<string> classes)
        {
            var results = new List<DetectionResult>();
            int numClasses = classes.Count;
            int numBoxes = output.Dimensions[2]; // 输出维度：[1, 4 + num_classes, num_boxes]


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
                for (int c = 0; c < numClasses; c++)
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

                // 计算padding（对齐官方scale_boxes）
                float gain = Math.Min(640 / originalWidth, 640 / originalHeight);
                float paddingX = (float)Math.Round((640 - (originalWidth * gain)) / 2 - 0.1);
                float paddingY = (float)Math.Round((640 - (originalHeight * gain)) / 2 - 0.1);

                // 直接去padding + 缩放到原图（无需×640，因为已是像素值）
                x1 = (x1 - paddingX) / gain;
                y1 = (y1 - paddingY) / gain;
                x2 = (x2 - paddingX) / gain;
                y2 = (y2 - paddingY) / gain;

                // 坐标范围限制（对齐官方clip_boxes）
                //x1 = Math.Clamp(x1, 0, originalWidth);
                //y1 = Math.Clamp(y1, 0, originalHeight);
                //x2 = Math.Clamp(x2, 0, originalWidth);
                //y2 = Math.Clamp(y2, 0, originalHeight);

                results.Add(new DetectionResult
                {
                    Label = classes[maxClassIdx],
                    Confidence = maxConf,
                    X1 = x1,
                    Y1 = y1,
                    X2 = x2,
                    Y2 = y2
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
        private List<DetectionResult> ApplyNMS(List<DetectionResult> detections, float iouThreshold)
        {
            var finalDetections = new List<DetectionResult>();
            // 按类别分组处理
            foreach (var group in detections.GroupBy(d => d.Label))
            {
                // 按置信度降序排序
                var sortedDetections = group.OrderByDescending(d => d.Confidence).ToList();
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
        private float CalculateIOU(DetectionResult a, DetectionResult b)
        {
            float intersectX1 = Math.Max(a.X1, b.X1);
            float intersectY1 = Math.Max(a.Y1, b.Y1);
            float intersectX2 = Math.Min(a.X2, b.X2);
            float intersectY2 = Math.Min(a.Y2, b.Y2);

            if (intersectX1 >= intersectX2 || intersectY1 >= intersectY2)
                return 0;

            float intersectArea = (intersectX2 - intersectX1) * (intersectY2 - intersectY1);
            float areaA = (a.X2 - a.X1) * (a.Y2 - a.Y1);
            float areaB = (b.X2 - b.X1) * (b.Y2 - b.Y1);

            return intersectArea / (areaA + areaB - intersectArea);
        }
    }
}
