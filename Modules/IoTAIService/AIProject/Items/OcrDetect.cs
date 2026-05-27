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
    public class OcrDetect : IDetect
    {
        private ITAServiceProvider _provider;
        public async Task Init(ITAServiceProvider provider)
        {
            _provider = provider;
            string tkey = "OcrDet";
            provider.GetService<AIProjectManager>().RegDetect(tkey, this);
            var redis = provider.GetService<GeneralRedisHelper>();
            await redis.HashSetAsync("AI-Items", tkey, new AIProjectInfo()
            {
                Name = "文本检测",
                Code = tkey,
                Stage = "Detect",
                Remark = "文本检测模块是OCR系统中的关键组成部分，负责在图像中定位和标记出包含文本的区域",
                ParamList = new List<AIProjectParam>()
                {
                    new AIProjectParam()
                    {
                        name="自动旋转",
                        code="isrotate",
                        type="boolean",
                        defval=false,
                        help="设置是否自动识别文档方向并旋转"
                    },
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

        public List<BoxItem> GenerateBoxs(Image<Rgb24> image, byte[] rawBytes, AIConfigData config)
        {
            bool tIsRotate = config.GetBool("isrotate", false);
            float tThreshold = config.GetFloat("threshold", 0.7f);
            Image<Rgb24> tmpImg;
            if (tIsRotate)
            {
                tmpImg = _provider.GetService<OcrDocOriRunner>().AutoRotate(image);
            }
            else
            {
                tmpImg = image;
            }
            List<BoxItem> boxes = _provider.GetService<OcrDetectRunner>().Predict(image, 0.3f, tThreshold);
            return boxes;
        }
    }
}
