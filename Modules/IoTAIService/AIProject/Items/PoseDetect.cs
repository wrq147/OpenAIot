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
    /// <summary>
    /// 人体姿势检测
    /// </summary>
    public class PoseDetect : IDetect
    {
        private ITAServiceProvider _provider;
        public async Task Init(ITAServiceProvider provider)
        {
            _provider = provider;
            string tkey = "Pose";
            provider.GetService<AIProjectManager>().RegDetect(tkey, this);
            var redis = provider.GetService<GeneralRedisHelper>();
            await redis.HashSetAsync("AI-Items", tkey, new AIProjectInfo()
            {
                Name = "人体姿势检测",
                Code = tkey,
                Stage = "Detect",
                Remark = "能够根据输入的图像或视频，检测出人体和人体关键点。",
                ParamList = new List<AIProjectParam>()
                {
                    new AIProjectParam()
                    {
                        name="检测阈值",
                        code="threshold",
                        type="float",
                        defval=0.7f,
                        min=0,
                        max=1,
                        help="0~1的区间值,值越小,对目标检测越模糊"
                    }
                }
            });
        }

        public List<BoxItem> GenerateBoxs(Image<Rgb24> image, AIConfigData config)
        {
            float tThreshold = config.GetFloat("threshold", 0.7f);
            List<BoxItem> boxes = _provider.GetService<YoloPoseDetectRunner>().Predict(image, tThreshold);
            return boxes;
        }
    }
}
