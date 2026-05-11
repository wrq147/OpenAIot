using ChannelUtility.Message;
using Common;
using Microsoft.ML.OnnxRuntime.Tensors;
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
        public override bool JudgeExe(AIConfigData config, bool needback)
        {
            bool tAllIn = config.GetBool("allin", false);
            if (tAllIn)
            {
                return true;
            }
            else
            {
                return needback;
            }
        }
        public override async Task Execute(AIDetectRequestMeesage req, Image<Rgb24> image, AIConfigData config, List<BoxItem> boxes)
        {
            bool tAllIn = config.GetBool("allin", false);
            List<object> feature = config.Get("feature") as List<object>;
            var aiCache = _provider.GetService<AICache>();
            var videoData = aiCache.GetVideoCache(req.DeviceId);
            var tracklist = videoData.TrackList;
            var addlist = videoData.AddTrackList;
            var imgfeature = ConvertListToDenseTensor(feature);
            if (tAllIn)
            {

            }
            else
            {
                foreach (var trackItem in tracklist)
                {

                }
            }
        }
        private DenseTensor<float> ConvertListToDenseTensor(List<object> data)
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

            // ---------------------- 创建DenseTensor ----------------------
            var tensorShape = new int[] { 1, data.Count };
            return new DenseTensor<float>(flatArray, tensorShape);
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
