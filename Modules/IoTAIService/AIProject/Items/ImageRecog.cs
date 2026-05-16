using ChannelUtility.Message;
using Common;
using IoTAIService.AICode;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace IoTAIService.AIProject.Items
{
    public class ImageRecog : Infer
    {
        private ITAServiceProvider _provider;
        public override async Task Execute(AIDetectRequestMeesage req, Image<Rgb24> image, AIConfigData config, List<BoxItem> boxes)
        {
            bool tAllIn = config.GetBool("allin", false);
            List<object> feature = config.Get("feature") as List<object>;
            var aiCache = _provider.GetService<AICache>();
            var videoData = aiCache.GetVideoCache(req.DeviceId);
            var tracklist = videoData.TrackList;
            var addlist = videoData.AddTrackList;
            var imgfeature = ConvertListToFloat(feature);

            if (tAllIn)
            {
                var targetImgFeature = _provider.GetService<MobileCLIP2VisionRunner>().OutputEmbeddings(image);
                float sim = CosineSimilarity(targetImgFeature, imgfeature);
            }
            else
            {
                foreach (var trackItem in addlist)
                {
                    var targetImgFeature = _provider.GetService<MobileCLIP2VisionRunner>().ExtractFeature(image, trackItem.CurrentDetection);
                    float sim = CosineSimilarity(targetImgFeature, imgfeature);
                }
            }
        }
        private float[] ConvertListToFloat(List<object> data)
        {
            // 空数据校验
            if (data == null || data.Count == 0)
            {
                throw new ArgumentException("输入数据不能为空", nameof(data));
            }
            float[] flatArray = new float[data.Count];
            int index = 0;
            foreach (var row in data)
            {
                flatArray[index++] = Convert.ToSingle(row);
            }
            return flatArray;
        }
        /// <summary>
        /// 计算两个 float[] 特征向量的余弦相似度
        /// 范围：[-1,1]，越接近1越相似
        /// </summary>
        public float CosineSimilarity(float[] vecA, float[] vecB)
        {
            // 长度必须一致
            if (vecA == null || vecB == null || vecA.Length != vecB.Length)
                return 0f;

            double dotProduct = 0.0;
            double normA = 0.0;
            double normB = 0.0;

            for (int i = 0; i < vecA.Length; i++)
            {
                dotProduct += vecA[i] * vecB[i];
                normA += vecA[i] * vecA[i];
                normB += vecB[i] * vecB[i];
            }

            double magA = Math.Sqrt(normA);
            double magB = Math.Sqrt(normB);

            if (magA == 0 || magB == 0)
                return 0f;

            var cos = (float)(dotProduct / (magA * magB));

            double score01 = (cos + 1.0) / 2.0;
            // 限制边界防止浮点溢出
            return (float)Math.Clamp(score01, 0.0, 1.0);
        }
        public override async Task Init(ITAServiceProvider provider)
        {
            _provider = provider;
            string tkey = "ImageRecog";
            provider.GetService<AIProjectManager>().RegInfer(tkey, this);
            var redis = provider.GetService<GeneralRedisHelper>();
            await redis.HashSetAsync("AI-Items", tkey, new AIProjectInfo()
            {
                Name = "图片识别",
                Code = tkey,
                Stage = "Infer",
                Remark = "提取检测目标图像特征并进行比对，判断检测目标与对比图像的相似度。",
                ParamList = new List<AIProjectParam>()
                {
                    new AIProjectParam()
                    {
                        name="识别整图",
                        code="allin",
                        type="boolean",
                        defval=false,
                        help="设置对比整图还是检测的目标"
                    },
                    new AIProjectParam()
                    {
                        name="对比图像",
                        code="feature",
                        type="imgclip",
                        help="必填项,用来生成对比图像的特征"
                    }
                }
            });
        }
    }
}
