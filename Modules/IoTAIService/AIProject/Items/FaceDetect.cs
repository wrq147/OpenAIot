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
    public class FaceDetect : IDetect
    {
        private ITAServiceProvider _provider;
        public async Task Init(ITAServiceProvider provider)
        {
            _provider = provider;
            string tkey = "FaceDetect";
            provider.GetService<AIProjectManager>().RegDetect(tkey, this);
            var redis = provider.GetService<GeneralRedisHelper>();
            await redis.HashSetAsync("AI-Items", tkey, new AIProjectInfo()
            {
                Name = "人脸检测",
                Code = tkey,
                Stage = "Detect",
                Remark = "检测定位人脸区域。",
                ParamList = new List<AIProjectParam>()
                {
                    new AIProjectParam()
                    {
                        name="人脸阈值",
                        code="threshold",
                        type="float",
                        defval=0.7f,
                        min=0,
                        max=1,
                        help="0~1的区间值,值越小,对人脸的检测越模糊"
                    },
                    new AIProjectParam()
                    {
                        name="交并阈值",
                        code="iou_threshold",
                        type="float",
                        defval=0.2f,
                        min=0,
                        max=1,
                        help="0~1的区间值,值越小,越不会检测重合人脸"
                    }
                }
            });
        }
        public List<BoxItem> GenerateBoxs(Image<Rgb24> image, byte[] rawBytes, AIConfigData config)
        {
            float tThreshold = config.GetFloat("threshold", 0.7f);
            float tIOU = config.GetFloat("iou_threshold", 0.2f);
            var tmpboxs = _provider.GetService<YoloFaceDetectRunner>().Predict(image, tThreshold, tIOU);
            return tmpboxs;
        }


    }
}
