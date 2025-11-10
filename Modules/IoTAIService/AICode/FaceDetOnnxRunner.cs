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
    public class FaceDetOnnxRunner
    {
        private readonly InferenceSession _session;

        public FaceDetOnnxRunner(IOptions<IoTAIOption> option)
        {
            // 初始化ONNX推理会话
            _session = new InferenceSession(option.Value.FaceDetectionFile);
        }

        public List<BBox> Predict(Image<Rgb24> image, float conf_threshold = 0.8f, float iou_threshold = 0.2f)
        {
            int[] strides = new int[3] { 4, 8, 16 };
            (var clsOutputs, var regOutputs) = PredictModel(image, strides.Length);
            var listbox = DecodePredictions(clsOutputs, regOutputs, strides, 320, 320, conf_threshold);
            if (listbox.Count > 0)
            {
                listbox = Nms(listbox, iou_threshold);
                listbox = ResizeBBoxes(listbox, 320, 320, image.Width, image.Height);
            }
            return listbox;
        }
        private (List<Tensor<float>> clsOutputs, List<Tensor<float>> regOutputs) PredictModel(Image<Rgb24> image, int scaleLen)
        {
            // 1. 图像预处理（与训练时保持一致）
            var inputTensor = PreprocessImage(image);

            // 2. 准备输入
            var inputs = new List<NamedOnnxValue> { NamedOnnxValue.CreateFromTensor("input", inputTensor) };

            // 3. 执行推理
            using var outputs = _session.Run(inputs);

            // 4. 解析输出（注意：输出格式需与ONNX导出时的定义对应）
            List<Tensor<float>> clsOutputs = new List<Tensor<float>>();
            List<Tensor<float>> regOutputs = new List<Tensor<float>>();
            for (int i = 0; i < scaleLen; i++)
            {
                var clsOutput = outputs.First(o => o.Name == ("cls_output_" + i)).AsTensor<float>();
                clsOutputs.Add(clsOutput);
                var regOutput = outputs.First(o => o.Name == ("reg_output_" + i)).AsTensor<float>();
                regOutputs.Add(regOutput);
            }


            return (clsOutputs, regOutputs);
        }
        // 图像预处理：缩放、归一化等
        private Tensor<float> PreprocessImage(Image<Rgb24> image)
        {
            // 缩放为320x320
            image.Mutate(x => x.Resize(320, 320));

            // 转换为张量
            var tensor = new DenseTensor<float>(new[] { 1, 3, 320, 320 });
            for (int y = 0; y < 320; y++)
            {
                for (int x = 0; x < 320; x++)
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


        /// <summary>
        /// 解码预测结果为边界框
        /// </summary>
        /// <param name="classifications">各尺度分类结果 (List[Tensor], 每个Tensor形状: [1, num_class, H, W])</param>
        /// <param name="regressions">各尺度回归结果 (List[Tensor], 每个Tensor形状: [1, 4, H, W])</param>
        /// <param name="strides">各尺度下采样步长</param>
        /// <param name="inputHeight">输入图像高度</param>
        /// <param name="inputWidth">输入图像宽度</param>
        /// <param name="confThreshold">置信度阈值</param>
        /// <returns>解码后的边界框列表</returns>
        private List<BBox> DecodePredictions(
            List<Tensor<float>> classifications,
            List<Tensor<float>> regressions,
            int[] strides,
            int inputHeight,
            int inputWidth,
            float confThreshold)
        {
            var bboxes = new List<BBox>();

            // 遍历每个尺度的特征图
            for (int scale = 0; scale < classifications.Count; scale++)
            {
                var clsTensor = classifications[scale];
                var regTensor = regressions[scale];
                int stride = strides[scale];

                // 获取特征图尺寸 (H, W)，注意Tensor维度为 [batch, channel, H, W]
                int batchSize = clsTensor.Dimensions[0];  // 固定为1（推理时通常单 batch）
                int numClasses = clsTensor.Dimensions[1]; // 类别数（这里假设单类别，取索引0）
                int featH = clsTensor.Dimensions[2];
                int featW = clsTensor.Dimensions[3];

                // 生成网格坐标 (x: 列索引, y: 行索引)
                for (int y = 0; y < featH; y++)
                {
                    for (int x = 0; x < featW; x++)
                    {
                        // 获取当前网格点的分类置信度（单类别场景取第0类）
                        float score = clsTensor[0, 0, y, x]; // [batch=0, class=0, y, x]
                        if (score < confThreshold)
                            continue; // 过滤低置信度

                        // 获取回归参数 (dx, dy, dw, dh)
                        float dx = regTensor[0, 0, y, x]; // [batch=0, dx, y, x]
                        float dy = regTensor[0, 1, y, x]; // [batch=0, dy, y, x]
                        float dw = regTensor[0, 2, y, x]; // [batch=0, dw, y, x]
                        float dh = regTensor[0, 3, y, x]; // [batch=0, dh, y, x]

                        // 计算中心点坐标 (cx, cy) = (网格坐标 + 偏移量) * 步长
                        float cx = (x + dx) * stride;
                        float cy = (y + dy) * stride;

                        // 计算宽高 (bw, bh) = exp(回归值) * 步长
                        float bw = (float)Math.Exp(dw) * stride;
                        float bh = (float)Math.Exp(dh) * stride;

                        // 转换为左上角和右下角坐标
                        float x1 = cx - bw / 2;
                        float y1 = cy - bh / 2;
                        float x2 = cx + bw / 2;
                        float y2 = cy + bh / 2;

                        // 裁剪坐标到图像范围内
                        x1 = Math.Clamp(x1, 0, inputWidth);
                        y1 = Math.Clamp(y1, 0, inputHeight);
                        x2 = Math.Clamp(x2, 0, inputWidth);
                        y2 = Math.Clamp(y2, 0, inputHeight);

                        // 添加到边界框列表
                        bboxes.Add(new BBox
                        {
                            X1 = x1,
                            Y1 = y1,
                            X2 = x2,
                            Y2 = y2,
                            Score = score
                        });
                    }
                }
            }

            return bboxes;
        }

        private List<BBox> Nms(List<BBox> bboxes, float iouThreshold)
        {
            if (bboxes.Count == 0)
                return new List<BBox>();

            var sortedBBoxes = bboxes.OrderByDescending(b => b.Score).ToList();
            var keep = new List<BBox>();

            while (sortedBBoxes.Count > 0)
            {
                var current = sortedBBoxes[0];
                keep.Add(current);
                sortedBBoxes.RemoveAt(0);

                sortedBBoxes = sortedBBoxes.Where(bbox =>
                    CalculateIou(current, bbox) < iouThreshold).ToList();
            }

            return keep;
        }

        private float CalculateIou(BBox box1, BBox box2)
        {
            float interX1 = Math.Max(box1.X1, box2.X1);
            float interY1 = Math.Max(box1.Y1, box2.Y1);
            float interX2 = Math.Min(box1.X2, box2.X2);
            float interY2 = Math.Min(box1.Y2, box2.Y2);

            float interArea = Math.Max(0, interX2 - interX1) * Math.Max(0, interY2 - interY1);
            float box1Area = (box1.X2 - box1.X1) * (box1.Y2 - box1.Y1);
            float box2Area = (box2.X2 - box2.X1) * (box2.Y2 - box2.Y1);

            return interArea / (box1Area + box2Area - interArea + 1e-8f);
        }

        /// <summary>
        /// 调整边界框到原始尺寸
        /// </summary>
        private List<BBox> ResizeBBoxes(List<BBox> bboxes,
            int inputWidth, int inputHeight, int originalWidth, int originalHeight)
        {
            float scaleW = (float)originalWidth / inputWidth;
            float scaleH = (float)originalHeight / inputHeight;

            return bboxes.Select(bbox => new BBox
            {
                X1 = bbox.X1 * scaleW,
                Y1 = bbox.Y1 * scaleH,
                X2 = bbox.X2 * scaleW,
                Y2 = bbox.Y2 * scaleH,
                Score = bbox.Score
            }).ToList();
        }
    }

    public class BBox
    {
        public float X1 { get; set; }
        public float Y1 { get; set; }
        public float X2 { get; set; }
        public float Y2 { get; set; }
        public float Score { get; set; }
    }
}
