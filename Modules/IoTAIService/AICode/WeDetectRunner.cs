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
    public class WeDetectRunner
    {
        private readonly InferenceSession _session;
        private readonly float[] _strides = new float[] { 8, 16, 32 };
        private readonly int[] _featSizes = new int[] { 80, 40, 20 };
        public WeDetectRunner()
        {
            // 初始化ONNX推理会话
            var sessionOptions = new SessionOptions();
            AIUtility.TryEnableGpu(sessionOptions);
            string modelName = "WeDetect.onnx";
            string modelPath = Directory.GetCurrentDirectory() + System.IO.Path.DirectorySeparatorChar + @"AIModel" + System.IO.Path.DirectorySeparatorChar + modelName;
            _session = new InferenceSession(modelPath, sessionOptions);
        }

        public List<BoxItem> Predict(Image<Rgb24> image, float confidenceThreshold, float iouThreshold, DenseTensor<float> classEmbeds, List<string> classes)
        {
            int originalWidth = image.Width;
            int originalHeight = image.Height;
            float gain = Math.Min(640.0f / originalWidth, 640.0f / originalHeight);
            // 1. 图像预处理（与训练时保持一致）
            var inputTensor = PreprocessImage(image, gain);
            // 2. 准备输入
            var inputs = new List<NamedOnnxValue> {    
                // 图片输入：假设已预处理为(1,3,640,640)的Tensor<float>
                 NamedOnnxValue.CreateFromTensor("image", inputTensor),
                // 文本嵌入输入：传入转换后的Tensor<float>
                NamedOnnxValue.CreateFromTensor("text_feats", classEmbeds)
            };

            // 3. 执行推理
            using var outputs = _session.Run(inputs);
            var cls80 = outputs.First(o => o.Name == "cls_80").AsTensor<float>();
            var reg80 = outputs.First(o => o.Name == "reg_80").AsTensor<float>();
            var cls40 = outputs.First(o => o.Name == "cls_40").AsTensor<float>();
            var reg40 = outputs.First(o => o.Name == "reg_40").AsTensor<float>();
            var cls20 = outputs.First(o => o.Name == "cls_20").AsTensor<float>();
            var reg20 = outputs.First(o => o.Name == "reg_20").AsTensor<float>();
            // 4. 后处理解析结果
            var detectionResults = PostprocessOutput(cls80, reg80, cls40, reg40, cls20, reg20, confidenceThreshold, iouThreshold, originalWidth, originalHeight, gain, classes);

            return detectionResults;
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

        private List<BoxItem> PostprocessOutput(
            Tensor<float> cls80, Tensor<float> reg80,
            Tensor<float> cls40, Tensor<float> reg40,
            Tensor<float> cls20, Tensor<float> reg20,
            float confThresh, float iouThresh, int oriW, int oriH, float gain, List<string> classes)
        {
            var allBoxes = new List<BoxItem>();

            // 解码 3 个尺度
            DecodeLevel(cls80, reg80, _strides[0], _featSizes[0], _featSizes[0], confThresh, allBoxes, classes);
            DecodeLevel(cls40, reg40, _strides[1], _featSizes[1], _featSizes[1], confThresh, allBoxes, classes);
            DecodeLevel(cls20, reg20, _strides[2], _featSizes[2], _featSizes[2], confThresh, allBoxes, classes);

            // NMS 非极大值抑制
            var nmsResult = ApplyNMS(allBoxes, iouThresh);

            foreach (var box in nmsResult)
            {
                RestoreCoords(box, oriW, oriH, gain);
            }

            return nmsResult;
        }
        private void RestoreCoords(BoxItem box, int oriW, int oriH, float gain)
        {
            float padX = (640 - oriW * gain) / 2f - 0.1f;
            float padY = (640 - oriH * gain) / 2f - 0.1f;

            float x1 = (box.x1 - padX) / gain;
            float y1 = (box.y1 - padY) / gain;
            float x2 = (box.x2 - padX) / gain;
            float y2 = (box.y2 - padY) / gain;

            x1 = Math.Clamp(x1, 0, oriW);
            y1 = Math.Clamp(y1, 0, oriH);
            x2 = Math.Clamp(x2, 0, oriW);
            y2 = Math.Clamp(y2, 0, oriH);
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
        private void DecodeLevel(
         Tensor<float> cls, Tensor<float> reg,
         float stride, int h, int w,
         float confThresh,
         List<BoxItem> results, List<string> classes)
        {
            int numClasses = cls.Dimensions[1];
            Span<float> ltrb = stackalloc float[4];
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    ltrb[0] = reg[0, 0, y, x] * stride;
                    ltrb[1] = reg[0, 1, y, x] * stride;
                    ltrb[2] = reg[0, 2, y, x] * stride;
                    ltrb[3] = reg[0, 3, y, x] * stride;


                    float cx = x * stride + stride * 0.5f;
                    float cy = y * stride + stride * 0.5f;
                    float x1 = cx - ltrb[0];
                    float y1 = cy - ltrb[1];
                    float x2 = cx + ltrb[2];
                    float y2 = cy + ltrb[3];


                    // 遍历所有类别得分
                    for (int c = 0; c < numClasses; c++)
                    {
                        float score = Sigmoid(cls[0, c, y, x]);
                        if (score < confThresh) continue;

                        results.Add(new BoxItem
                        {
                            x1 = x1,
                            y1 = y1,
                            x2 = x2,
                            y2 = y2,
                            score = score,
                            label = classes[c],
                            color = "#FF0000"
                        });
                    }
                }
            }
        }

        private float Sigmoid(float x) => 1.0f / (1.0f + MathF.Exp(-x));
    }
}
