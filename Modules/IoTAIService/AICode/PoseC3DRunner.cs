using ChannelUtility.Message;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using NPOI.HPSF;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTAIService.AICode
{
    public class PoseC3DRunner
    {
        public PoseC3DRunner()
        {

        }
        public (int classId, string className, float score) Process(List<BoxItem> poseFrames, float minKpScore = 0.3f)
        {
            var inferenceSession = InferenceSessionPool.Instance.GetInferenceSession(nameof(PoseC3DRunner), () =>
            {
                string modelPath = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + @"AIModel" + Path.DirectorySeparatorChar + "Posec3d.onnx";
                // 初始化ONNX推理会话
                var sessionOptions = new SessionOptions();
                AIUtility.TryEnableGpu(sessionOptions);
                return new InferenceSession(modelPath, sessionOptions);
            });
            try
            {
                Tensor<float> inputTensor = ConvertToPoseC3DTensor(poseFrames, minKpScore);
                var inputs = new List<NamedOnnxValue> {
                 NamedOnnxValue.CreateFromTensor("input_tensor", inputTensor),
            };

                // 执行推理
                using var outputs = inferenceSession.Run(inputs);
                var outputTensor = outputs.First().AsTensor<float>();

                // 输出是 [1,120]，取第 0 个 batch 的 120 维分数
                float[] scores = outputTensor.Skip(0).Take(120).ToArray();
                int classId = ArgMax(scores);
                string className = GetNTU120ActionName(classId);
                float score = scores[classId];
                // 用 Sigmoid 把 score 压缩到 0~1
                score = 1.0f / (1.0f + (float)Math.Exp(-score));
                return (classId, className, score);
            }
            finally
            {
                InferenceSessionPool.Instance.ReleaseSession(nameof(PoseC3DRunner), inferenceSession);
            }

        }

        /// <summary>
        /// 取数组最大值索引
        /// </summary>
        private int ArgMax(float[] input)
        {
            int index = 0;
            float max = input[0];
            for (int i = 1; i < input.Length; i++)
            {
                if (input[i] > max)
                {
                    max = input[i];
                    index = i;
                }
            }
            return index;
        }
        public static string[] NTU120_Actions_Chinese = new[]
         {
            "喝水","吃饭/吃零食","刷牙","梳头","掉落","捡起","扔东西","坐下","从坐姿站起","鼓掌",
            "阅读","写字","撕纸","穿外套","脱外套","穿鞋","脱鞋","戴眼镜","摘眼镜","戴帽子",
            "摘帽子","加油/振奋","挥手","踢东西","伸手进口袋","单脚跳","向上跳","打电话/接电话","玩手机/平板","敲击键盘",
            "用手指指向","自拍","看手表时间","搓手","点头/鞠躬","摇头","擦脸","敬礼","双手合十","双手交叉示意停止",
            "打喷嚏/咳嗽","摇摇晃晃","摔倒","摸头（头疼）","摸胸口（胃疼/心疼）","摸后背（背疼）","摸脖子（脖子疼）","恶心/想吐","扇风/感觉热","打/扇别人",
            "踢别人","推别人","拍别人后背","用手指指着别人","拥抱别人","给别人东西","摸别人口袋","握手","互相走近","互相走开",
            "戴耳机","摘耳机","投篮","拍球","挥网球拍","颠乒乓球","嘘（安静）","撩头发","竖大拇指","大拇指向下",
            "比OK手势","比胜利手势","装订书本","数钱","剪指甲","剪纸（用剪刀）","打响指","开瓶子","闻/嗅","蹲下",
            "抛硬币","折纸","把纸揉成团","玩魔方","往脸上涂护肤品","往手背上涂护肤品","背包","卸包","把东西放进包里","从包里拿出东西",
            "打开盒子","搬重物","挥拳头","抛起帽子","举起双手","双臂交叉","手臂画圈","手臂摆动","原地跑","向后踢（踢屁股）",
            "交叉触脚","侧踢","打哈欠","伸展身体","擤鼻子","用东西打别人","用刀挥向别人","撞倒别人（身体撞击）","抢夺别人物品","用枪瞄准别人",
            "踩脚","击掌","干杯喝酒","与他人一起搬运","给别人拍照","跟随别人","耳语","交换物品","搀扶他人","猜拳"
        };
        public string GetNTU120ActionName(int classId)
        {
            if (classId < 0 || classId >= NTU120_Actions_Chinese.Length)
                return "未知动作";

            return NTU120_Actions_Chinese[classId];
        }
        private Tensor<float> ConvertToPoseC3DTensor(List<BoxItem> poseFrames, float minKpScore)
        {
            // 固定参数
            const int batch = 1;
            const int numJoints = 17;
            const int numFrames = 48;
            const int heatmapSize = 64;
            const float sigma = 2.0f;

            // 创建 5 维 Tensor：[batch, joints, frames, height, width]
            var tensor = new DenseTensor<float>(new[] { batch, numJoints, numFrames, heatmapSize, heatmapSize });

            // 遍历 48 帧
            for (int f = 0; f < numFrames; f++)
            {
                if (f >= poseFrames.Count) break;
                var frame = poseFrames[f];
                if (frame.points == null || frame.points.Count < 17) continue;

                // 遍历 17 个关键点
                for (int j = 0; j < numJoints; j++)
                {
                    var kp = frame.points[j];
                    if (kp.score < minKpScore) continue;

                    // 坐标归一化 → 映射到 64x64
                    int x = (int)(kp.x * heatmapSize);
                    int y = (int)(kp.y * heatmapSize);

                    x = Math.Clamp(x, 0, heatmapSize - 1);
                    y = Math.Clamp(y, 0, heatmapSize - 1);

                    // 绘制高斯热图
                    DrawGaussian(tensor, 0, j, f, x, y, sigma);
                }
            }

            return tensor;
        }

        /// <summary>
        /// 向 Tensor 指定位置绘制高斯热图
        /// </summary>
        private void DrawGaussian(Tensor<float> tensor, int b, int j, int f, int cx, int cy, float sigma)
        {
            int radius = (int)Math.Ceiling(sigma * 3);
            float sigma2 = sigma * sigma;
            float twoSigma2 = 2 * sigma2;

            int minX = Math.Max(0, cx - radius);
            int maxX = Math.Min(63, cx + radius);
            int minY = Math.Max(0, cy - radius);
            int maxY = Math.Min(63, cy + radius);

            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    float dx = x - cx;
                    float dy = y - cy;
                    float g = (float)Math.Exp(-(dx * dx + dy * dy) / twoSigma2);
                    tensor[b, j, f, y, x] = g;
                }
            }
        }
    }
}
