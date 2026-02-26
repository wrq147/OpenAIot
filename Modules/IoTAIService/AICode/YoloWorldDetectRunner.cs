using Microsoft.Extensions.Options;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
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

        public YoloWorldDetectRunner(IOptions<IoTAIOption> option)
        {
            // 初始化ONNX推理会话
            var sessionOptions = new SessionOptions();
            AIUtility.TryEnableGpu(sessionOptions);
            _session = new InferenceSession(option.Value.YoloWorldFile, sessionOptions);
        }

        public List<DetectionResult> Predict(Image<Rgb24> image, float confidenceThreshold, float iouThreshold, DenseTensor<float> classEmbeds, List<string> classes)
        {
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
            var detectionResults = PostprocessOutput(outputTensor, confidenceThreshold, iouThreshold, scaleX, scaleY, classes);

            return detectionResults;
        }
        // 图像预处理：缩放、归一化等
        private Tensor<float> PreprocessImage(Image<Rgb24> image, out float scaleX, out float scaleY)
        {
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
        /// <param name="classes"></param>
        /// <returns></returns>
        private List<DetectionResult> PostprocessOutput(Tensor<float> output, float confidenceThreshold, float iouThreshold, float scaleX, float scaleY, List<string> classes)
        {
            var results = new List<DetectionResult>();
            int numClasses = classes.Count;
            int numBoxes = output.Dimensions[1]; // 输出维度：[1, num_boxes, 4 + num_classes]

            // 解析每个检测框
            for (int i = 0; i < numBoxes; i++)
            {
                // 检测框坐标（xyxy格式，模型输出是归一化到0-1的坐标）
                float x1 = output[0, i, 0] * 640;
                float y1 = output[0, i, 1] * 640;
                float x2 = output[0, i, 2] * 640;
                float y2 = output[0, i, 3] * 640;

                // 遍历所有类别，获取最高置信度的类别
                float maxConf = 0;
                int maxClassIdx = -1;
                for (int c = 0; c < numClasses; c++)
                {
                    float conf = output[0, i, 4 + c];
                    if (conf > maxConf)
                    {
                        maxConf = conf;
                        maxClassIdx = c;
                    }
                }

                // 过滤低置信度结果
                if (maxConf < confidenceThreshold || maxClassIdx == -1)
                    continue;

                // 还原检测框到原图坐标（扣除填充的黑边，再缩放）
                float paddingX = (640 - (x2 - x1) * scaleX / scaleX) / 2; // 简化：实际需根据缩放后的尺寸计算填充
                float paddingY = (640 - (y2 - y1) * scaleY / scaleY) / 2;
                x1 = (x1 - paddingX) * scaleX;
                y1 = (y1 - paddingY) * scaleY;
                x2 = (x2 - paddingX) * scaleX;
                y2 = (y2 - paddingY) * scaleY;

                // 确保坐标在原图范围内
                x1 = Math.Clamp(x1, 0, float.MaxValue);
                y1 = Math.Clamp(y1, 0, float.MaxValue);
                x2 = Math.Clamp(x2, 0, float.MaxValue);
                y2 = Math.Clamp(y2, 0, float.MaxValue);

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
