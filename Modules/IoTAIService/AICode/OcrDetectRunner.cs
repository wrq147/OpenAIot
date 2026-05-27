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
    public class OcrDetectRunner
    {
        public OcrDetectRunner()
        {
        }

        public List<BoxItem> Predict(Image<Rgb24> image, float maskThreshold = 0.3f, float scoreThresh = 0.7f, float unclipRatio = 1.5f)
        {
            var inferenceSession = InferenceSessionPool.Instance.GetInferenceSession(nameof(OcrDetectRunner), () =>
            {
                // 初始化ONNX推理会话
                var sessionOptions = new SessionOptions();
                AIUtility.TryEnableGpu(sessionOptions);
                string modelName = "OcrDet.onnx";
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
                    NamedOnnxValue.CreateFromTensor("x", inputTensor)
                };

                // 3. 执行推理
                using var outputs = inferenceSession.Run(inputs);
                var outputTensor = outputs.First().AsTensor<float>();
                // 4. 后处理解析结果
                var detectionResults = PostprocessOutput(outputTensor, maskThreshold, scoreThresh, unclipRatio, originalWidth, originalHeight, gain);

                return detectionResults;
            }
            finally
            {
                InferenceSessionPool.Instance.ReleaseSession(nameof(OcrDetectRunner), inferenceSession);
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
        /// 后处理：解析模型输出
        /// </summary>
        /// <param name="output"></param>
        /// <param name="maskThreshold"></param>
        /// <param name="scoreThresh"></param>
        /// <param name="unclipRatio"></param>
        /// <param name="originalWidth"></param>
        /// <param name="originalHeight"></param>
        /// <param name="gain"></param>
        /// <returns></returns>
        private List<BoxItem> PostprocessOutput(Tensor<float> output, float maskThreshold, float scoreThresh, float unclipRatio, int originalWidth, int originalHeight, float gain)
        {
            var results = new List<BoxItem>();
            int outH = output.Dimensions[2];
            int outW = output.Dimensions[3];
            float[] outputData = output.ToArray();
            bool[] binary = outputData.Select(v => v > maskThreshold).ToArray();

            var contours = FindContours(binary, outW, outH);
            var boxItems = new List<BoxItem>();

            float resizedW = originalWidth * gain;
            float resizedH = originalHeight * gain;
            float leftPad = (640 - resizedW) / 2f;
            float topPad = (640 - resizedH) / 2f;
            foreach (var contour in contours)
            {
                // 最小外接矩形 → 直接算 x1/y1/x2/y2
                var rect = GetBoundingRect(contour);
                float boxW = rect.x2 - rect.x1;
                float boxH = rect.y2 - rect.y1;
                if (Math.Min(boxW, boxH) < 3)
                    continue;

                // 置信度
                float score = BoxScoreFast(outputData, outW, outH, rect);
                if (score < scoreThresh)
                    continue;

                // 外扩
                var expanded = UnclipRect(rect, unclipRatio);
                var expRect = GetBoundingRectFromRect(expanded);
                float expW = expRect.x2 - expRect.x1;
                float expH = expRect.y2 - expRect.y1;
                if (Math.Min(expW, expH) < 5)
                    continue;

                float x1 = (expRect.x1 - leftPad) / gain;
                float y1 = (expRect.y1 - topPad) / gain;
                float x2 = (expRect.x2 - leftPad) / gain;
                float y2 = (expRect.y2 - topPad) / gain;

                // 裁剪到图片范围内
                x1 = Math.Clamp(x1, 0, originalWidth - 1);
                y1 = Math.Clamp(y1, 0, originalHeight - 1);
                x2 = Math.Clamp(x2, 0, originalWidth - 1);
                y2 = Math.Clamp(y2, 0, originalHeight - 1);

                float finalW = x2 - x1;
                float finalH = y2 - y1;
                if (finalW <= 2f || finalH <= 2f)
                    continue;

                boxItems.Add(new BoxItem
                {
                    x1 = x1,
                    y1 = y1,
                    x2 = x2,
                    y2 = y2,
                    score = (float)Math.Round(score, 4),
                    label = "文本",
                    color = "#A1A100"
                });
            }

            // 排序：从上到下 → 从左到右
            return boxItems.OrderBy(b => b.y1).ThenBy(b => b.x1).ToList();
        }
        private List<(float x, float y)> UnclipRect(BoxItem rect, float ratio)
        {
            var pts = new List<(float x, float y)>
            {
                (rect.x1, rect.y1), (rect.x2, rect.y1),
                (rect.x2, rect.y2), (rect.x1, rect.y2)
            };
            double area = (rect.x2 - rect.x1) * (rect.y2 - rect.y1);
            double length = 2 * ((rect.x2 - rect.x1) + (rect.y2 - rect.y1));
            double dist = area * ratio / (length + 1e-6);

            var res = new List<(float x, float y)>();
            foreach (var (x, y) in pts)
            {
                res.Add((
                    x + (x < (rect.x1 + rect.x2) / 2 ? -(float)dist : (float)dist),
                    y + (y < (rect.y1 + rect.y2) / 2 ? -(float)dist : (float)dist)
                ));
            }
            return res;
        }
        private BoxItem GetBoundingRect(List<(float x, float y)> pts)
        {
            float minX = pts.Min(p => p.x);
            float maxX = pts.Max(p => p.x);
            float minY = pts.Min(p => p.y);
            float maxY = pts.Max(p => p.y);
            return new BoxItem { x1 = minX, y1 = minY, x2 = maxX, y2 = maxY };
        }

        // 外扩后的四点 → 再算外接矩形
        private BoxItem GetBoundingRectFromRect(List<(float x, float y)> pts)
        {
            float minX = pts.Min(p => p.x);
            float maxX = pts.Max(p => p.x);
            float minY = pts.Min(p => p.y);
            float maxY = pts.Max(p => p.y);
            return new BoxItem { x1 = minX, y1 = minY, x2 = maxX, y2 = maxY };
        }
        private float BoxScoreFast(float[] pred, int w, int h, BoxItem rect)
        {
            int x1 = (int)Math.Clamp(rect.x1, 0, w - 1);
            int x2 = (int)Math.Clamp(rect.x2, 0, w - 1);
            int y1 = (int)Math.Clamp(rect.y1, 0, h - 1);
            int y2 = (int)Math.Clamp(rect.y2, 0, h - 1);

            float sum = 0;
            int cnt = 0;
            for (int y = y1; y <= y2; y++)
                for (int x = x1; x <= x2; x++)
                {
                    sum += pred[y * w + x];
                    cnt++;
                }
            return cnt > 0 ? sum / cnt : 0;
        }
        private List<List<(float x, float y)>> FindContours(bool[] binary, int w, int h)
        {
            bool[] visited = new bool[binary.Length];
            var contours = new List<List<(float x, float y)>>();
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    int i = y * w + x;
                    if (binary[i] && !visited[i])
                    {
                        var c = new List<(float x, float y)>();
                        Dfs(x, y, w, h, binary, visited, c);
                        if (c.Count > 0) contours.Add(c);
                    }
                }
            }
            return contours;
        }
        private void Dfs(int x, int y, int w, int h, bool[] bin, bool[] v, List<(float x, float y)> c)
        {
            if (x < 0 || y < 0 || x >= w || y >= h) return;
            int i = y * w + x;
            if (!bin[i] || v[i]) return;
            v[i] = true;
            c.Add((x, y));
            Dfs(x + 1, y, w, h, bin, v, c);
            Dfs(x - 1, y, w, h, bin, v, c);
            Dfs(x, y + 1, w, h, bin, v, c);
            Dfs(x, y - 1, w, h, bin, v, c);
        }
    }
}
