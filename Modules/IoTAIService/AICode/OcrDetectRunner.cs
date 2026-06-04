using ChannelUtility.Message;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using Quartz.Impl.AdoJobStore.Common;
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
    public class OcrDetectRunner
    {
        private const float Epsilon = 1e-6f;
        private const float MaxLen = 640.0f;
        public OcrDetectRunner()
        {
        }

        public List<BoxItem> Predict(Image<Rgb24> image, float maskThreshold = 0.3f, float scoreThresh = 0.6f, float unclipRatio = 1.5f)
        {
            var inferenceSession = InferenceSessionPool.Instance.GetInferenceSession(nameof(OcrDetectRunner), () =>
            {
                // 初始化ONNX推理会话
                var sessionOptions = new SessionOptions();
                var provider = AIUtility.TryEnableGpu(sessionOptions);
                string modelName = "OcrDet.onnx";
                if (provider != ExecutionProviderType.CPU)
                {
                    modelName = "OcrDetBig.onnx";
                }
                string modelPath = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + @"AIModel" + Path.DirectorySeparatorChar + modelName;
                return new InferenceSession(modelPath, sessionOptions);
            });
            try
            {
                int originalWidth = image.Width;
                int originalHeight = image.Height;
                float gain = Math.Min(MaxLen / originalWidth, MaxLen / originalHeight);
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
            using Image<Rgb24> image = input.Clone();
            int originalWidth = image.Width;
            int originalHeight = image.Height;
            // 计算缩放比例（保持宽高比，填充黑边）
            float resizedWidth = originalWidth * gain;
            float resizedHeight = originalHeight * gain;
            float dw = (MaxLen - resizedWidth) / 2.0f;
            float dh = (MaxLen - resizedHeight) / 2.0f;
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
            int tmplen = (int)MaxLen;
            using Image<Rgb24> paddedImg = new Image<Rgb24>(tmplen, tmplen);
            paddedImg.Mutate(x =>
            {
                // 填充背景色
                x.Fill(new Rgb24(0, 0, 0));
                // 粘贴缩放后的图片（top/left为填充量）
                x.DrawImage(image, new Point(left, top), 1.0f);
            });

            // 转换为张量
            var tensor = new DenseTensor<float>(new[] { 1, 3, tmplen, tmplen });
            for (int y = 0; y < tmplen; y++)
            {
                for (int x = 0; x < tmplen; x++)
                {
                    var pixel = paddedImg[x, y];
                    // 归一化
                    tensor[0, 0, y, x] = (pixel.R / 255f - 0.485f) / 0.229f; // R
                    tensor[0, 1, y, x] = (pixel.G / 255f - 0.456f) / 0.224f; // G
                    tensor[0, 2, y, x] = (pixel.B / 255f - 0.406f) / 0.225f; // B
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
            float leftPad = (MaxLen - resizedW) / 2f;
            float topPad = (MaxLen - resizedH) / 2f;
            foreach (var contour in contours)
            {
                var boxPts = GetFastMinBox(contour);
                float w = Distance(boxPts[0], boxPts[1]);
                float h = Distance(boxPts[1], boxPts[2]);
                float side = MathF.Min(w, h);
                if (side < 3) continue;

                // 置信度
                float score = BoxScoreFast(outputData, outW, outH, boxPts);
                if (score < scoreThresh)
                    continue;

                // 外扩
                var finalPts = Unclip(boxPts, unclipRatio);

                float minX = finalPts.Min(p => p.X);
                float minY = finalPts.Min(p => p.Y);
                float maxX = finalPts.Max(p => p.X);
                float maxY = finalPts.Max(p => p.Y);

                float x1 = (minX - leftPad) / gain;
                float y1 = (minY - topPad) / gain;
                float x2 = (maxX - leftPad) / gain;
                float y2 = (maxY - topPad) / gain;

                // 裁剪到图片范围内
                x1 = Math.Clamp(x1, 0, originalWidth - 1);
                y1 = Math.Clamp(y1, 0, originalHeight - 1);
                x2 = Math.Clamp(x2, 0, originalWidth - 1);
                y2 = Math.Clamp(y2, 0, originalHeight - 1);

                float finalW = x2 - x1;
                float finalH = y2 - y1;
                if (finalW <= 2f || finalH <= 2f)
                    continue;

                var tmpbox = new BoxItem
                {
                    x1 = x1,
                    y1 = y1,
                    x2 = x2,
                    y2 = y2,
                    score = (float)Math.Round(score, 4),
                    label = "文本",
                    color = "#A1A100",
                    points = new List<KeyPoint>()
                };
                foreach(var pt in finalPts)
                {
                    tmpbox.points.Add(new KeyPoint() { x = pt.X, y = pt.Y });
                }
                boxItems.Add(tmpbox);
            }

            // 排序：从上到下 → 从左到右
            return boxItems.OrderBy(b => b.y1).ThenBy(b => b.x1).ToList();
        }
        private float Distance(PointF a, PointF b)
        {
            float dx = a.X - b.X;
            float dy = a.Y - b.Y;
            return MathF.Sqrt(dx * dx + dy * dy);
        }
        private List<PointF> Unclip(PointF[] box, float unclipRatio)
        {
            int ptsCount = box.Length;
            float[] x = box.Select(p => p.X).ToArray();
            float[] y = box.Select(p => p.Y).ToArray();

            // 1. 计算轮廓面积
            float area = 0;
            for (int i = 0; i < ptsCount; i++)
            {
                int j = (i + 1) % ptsCount;
                area += x[i] * y[j] - x[j] * y[i];
            }
            area = MathF.Abs(area) * 0.5f;

            // 2. 计算轮廓周长
            float length = 0;
            float[] edgeDx = new float[ptsCount];
            float[] edgeDy = new float[ptsCount];
            float[] edgeLen = new float[ptsCount];
            for (int i = 0; i < ptsCount; i++)
            {
                int j = (i + 1) % ptsCount;
                edgeDx[i] = x[j] - x[i];
                edgeDy[i] = y[j] - y[i];
                edgeLen[i] = MathF.Sqrt(edgeDx[i] * edgeDx[i] + edgeDy[i] * edgeDy[i]);
                length += edgeLen[i];
            }

            // 3. 计算偏移距离
            float distance = area * unclipRatio / (length + Epsilon);

            // 4. 计算边单位法向量 (dy, -dx)
            float[] normDx = new float[ptsCount];
            float[] normDy = new float[ptsCount];
            for (int i = 0; i < ptsCount; i++)
            {
                float len = edgeLen[i] + Epsilon;
                normDx[i] = edgeDy[i] / len;
                normDy[i] = -edgeDx[i] / len;
            }

            // 5. 顶点向量 = 当前边法向量 + 上一条边法向量
            float[] vx = new float[ptsCount];
            float[] vy = new float[ptsCount];
            for (int i = 0; i < ptsCount; i++)
            {
                int prev = (i - 1 + ptsCount) % ptsCount;
                vx[i] = normDx[i] + normDx[prev];
                vy[i] = normDy[i] + normDy[prev];
            }

            // 6. 计算夹角余弦 + 缩放系数，限制最大2倍
            float[] scale = new float[ptsCount];
            for (int i = 0; i < ptsCount; i++)
            {
                int prev = (i - 1 + ptsCount) % ptsCount;
                float dot = normDx[i] * normDx[prev] + normDy[i] * normDy[prev];
                float cosTheta = dot;
                float s = MathF.Sqrt(2.0f / (1.0f + cosTheta + Epsilon));
                if (s < 0) s = 0;
                if (s > 2) s = 2;
                scale[i] = s;
            }

            // 7. 顶点平移，得到外扩后坐标
            List<PointF> expanded = new List<PointF>(ptsCount);
            for (int i = 0; i < ptsCount; i++)
            {
                float move = distance * scale[i];
                expanded.Add(new PointF(
                    box[i].X + vx[i] * move,
                    box[i].Y + vy[i] * move
                ));
            }
            return expanded;
        }


        private float BoxScoreFast(float[] mask, int width, int height, PointF[] contourBox)
        {
            if (contourBox == null || contourBox.Length < 3)
                return 0;

            // 1. 获取轮廓外接矩形
            float minX = contourBox.Min(p => p.X);
            float maxX = contourBox.Max(p => p.X);
            float minY = contourBox.Min(p => p.Y);
            float maxY = contourBox.Max(p => p.Y);

            int xStart = (int)Math.Clamp(minX, 0, width - 1);
            int xEnd = (int)Math.Clamp(maxX, 0, width - 1);
            int yStart = (int)Math.Clamp(minY, 0, height - 1);
            int yEnd = (int)Math.Clamp(maxY, 0, height - 1);

            if (xStart >= xEnd || yStart >= yEnd)
                return 0;

            float sum = 0;
            int count = 0;

            // 2. 遍历矩形内所有点 → 只统计【在轮廓内部】的点
            for (int y = yStart; y <= yEnd; y++)
            {
                for (int x = xStart; x <= xEnd; x++)
                {
                    // ✅ 关键：判断点是否在多边形内部
                    if (IsPointInPolygon(new PointF(x, y), contourBox))
                    {
                        int index = y * width + x; // ✅ 正确索引
                        sum += mask[index];
                        count++;
                    }
                }
            }

            return count > 0 ? sum / count : 0;
        }

        /// <summary>
        /// 射线法判断点是否在多边形内
        /// </summary>
        private bool IsPointInPolygon(PointF p, PointF[] polygon)
        {
            bool inside = false;
            for (int i = 0, j = polygon.Length - 1; i < polygon.Length; j = i++)
            {
                if (((polygon[i].Y > p.Y) != (polygon[j].Y > p.Y)) &&
                    (p.X < (polygon[j].X - polygon[i].X) * (p.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) + polygon[i].X))
                {
                    inside = !inside;
                }
            }
            return inside;
        }

        private PointF[] GetFastMinBox(List<PointF> contour)
        {
            if (contour == null || contour.Count < 2)
                return Array.Empty<PointF>();

            // 1. 计算中心点
            float cx = contour.Average(p => p.X);
            float cy = contour.Average(p => p.Y);

            // 2. 构建协方差矩阵
            float xx = 0, yy = 0, xy = 0;
            int n = contour.Count;
            foreach (var p in contour)
            {
                float px = p.X - cx;
                float py = p.Y - cy;
                xx += px * px;
                yy += py * py;
                xy += px * py;
            }

            // 3. 特征值求主方向（OpenCV 算法）
            float u = xx + yy;
            float v = (float)Math.Sqrt((xx - yy) * (xx - yy) + 4 * xy * xy);
            float lambda1 = (u + v) * 0.5f; // 最大特征值
            float lambda2 = (u - v) * 0.5f; // 最小特征值

            float dx = 0, dy = 0;
            if (Math.Abs(xy) > 0.0001f)
            {
                dx = lambda1 - yy;
                dy = xy;
            }
            else
            {
                if (xx > yy)
                {
                    dx = 1; dy = 0;
                }
                else
                {
                    dx = 0; dy = 1;
                }
            }

            // 归一化方向向量
            float mag = (float)Math.Sqrt(dx * dx + dy * dy);
            if (mag > 0.0001f)
            {
                dx /= mag;
                dy /= mag;
            }

            // 垂直方向
            float vx = -dy;
            float vy = dx;

            // 4. 投影到两个主轴求 min/max
            float minMain = float.MaxValue, maxMain = float.MinValue;
            float minSide = float.MaxValue, maxSide = float.MinValue;

            foreach (var p in contour)
            {
                float px = p.X - cx;
                float py = p.Y - cy;
                float projMain = px * dx + py * dy;
                float projSide = px * vx + py * vy;

                minMain = Math.Min(minMain, projMain);
                maxMain = Math.Max(maxMain, projMain);
                minSide = Math.Min(minSide, projSide);
                maxSide = Math.Max(maxSide, projSide);
            }

            // 5. 计算四个角点（完全和 boxPoints 顺序一致）
            PointF[] box = new PointF[4];
            box[0] = new PointF(cx + minMain * dx + minSide * vx, cy + minMain * dy + minSide * vy);
            box[1] = new PointF(cx + maxMain * dx + minSide * vx, cy + maxMain * dy + minSide * vy);
            box[2] = new PointF(cx + maxMain * dx + maxSide * vx, cy + maxMain * dy + maxSide * vy);
            box[3] = new PointF(cx + minMain * dx + maxSide * vx, cy + minMain * dy + maxSide * vy);

            return box;
        }
        /// <summary>
        /// 输出轮廓点顺序、结构、简化规则完全一致
        /// </summary>
        private List<List<PointF>> FindContours(bool[] bin, int width, int height)
        {
            var contours = new List<List<PointF>>();
            bool[] visited = new bool[bin.Length];
            int[] dx = { -1, 0, 1, -1, 1, -1, 0, 1 };  // 8方向
            int[] dy = { -1, -1, -1, 0, 0, 1, 1, 1 };

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int idx = y * width + x;
                    if (bin[idx] && !visited[idx])
                    {
                        // 开始跟踪轮廓
                        List<Point> contour = TraceContour(x, y, bin, visited, width, height, dx, dy);
                        List<Point> simplified = ApproxContourSimple(contour);
                        if (simplified.Count >= 4)
                        {
                            contours.Add(simplified.Select(p => new PointF(p.X, p.Y)).ToList());
                        }
                    }
                }
            }

            return contours;
        }

        /// <summary>
        /// 轮廓跟踪
        /// </summary>
        private List<Point> TraceContour(int startX, int startY, bool[] bin, bool[] visited, int w, int h, int[] dx, int[] dy)
        {
            List<Point> contour = new List<Point>();
            Queue<Point> queue = new Queue<Point>();
            queue.Enqueue(new Point(startX, startY));
            visited[startY * w + startX] = true;

            while (queue.Count > 0)
            {
                Point p = queue.Dequeue();
                contour.Add(p);

                for (int d = 0; d < 8; d++)
                {
                    int nx = p.X + dx[d];
                    int ny = p.Y + dy[d];
                    if (nx >= 0 && ny >= 0 && nx < w && ny < h)
                    {
                        int nid = ny * w + nx;
                        if (bin[nid] && !visited[nid])
                        {
                            visited[nid] = true;
                            queue.Enqueue(new Point(nx, ny));
                        }
                    }
                }
            }

            return contour;
        }

        /// <summary>
        /// 去掉水平/垂直/对角线上的冗余中间点，只保留拐点
        /// </summary>
        private List<Point> ApproxContourSimple(List<Point> contour)
        {
            if (contour.Count <= 2) return contour;
            List<Point> res = new List<Point>();
            res.Add(contour[0]);

            for (int i = 1; i < contour.Count - 1; i++)
            {
                Point a = res.Last();
                Point b = contour[i];
                Point c = contour[i + 1];

                // 判断三点是否共线（水平、垂直、对角线）→ 是则跳过中间点
                if ((b.X - a.X) * (c.Y - a.Y) == (b.Y - a.Y) * (c.X - a.X))
                    continue;

                res.Add(b);
            }

            res.Add(contour.Last());
            return res;
        }

    }
}
