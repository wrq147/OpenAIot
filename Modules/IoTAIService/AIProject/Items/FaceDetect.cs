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
                        defval=0.8f,
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
        public List<BoxItem> GenerateBoxs(Image<Rgb24> image, AIConfigData config)
        {
            float tThreshold = config.GetFloat("threshold", 0.8f);
            float tIOU = config.GetFloat("iou_threshold", 0.2f);
            List<BoxItem> tmpboxs = new List<BoxItem>();
            var tbbx = _provider.GetService<FaceDetOnnxRunner>().Predict(image, tThreshold, tIOU);
            for (int i = 0; i < tbbx.Count; i++)
            {
                var titem = tbbx[i];
                tmpboxs.Add(new BoxItem()
                {
                    x1 = titem.X1,
                    x2 = titem.X2,
                    y1 = titem.Y1,
                    y2 = titem.Y2,
                    score = titem.Score,
                    label = "人脸",
                    color = "#67C23A"
                });
            }
            return tmpboxs;
        }


    }
}
