using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace IoTAIService.AIProject.Items
{
    public static class FaceProject
    {
        public static async Task Init(ITAServiceProvider provider)
        {
            var redis = provider.GetService<GeneralRedisHelper>();
            await redis.HashSetAsync("AI-Items", "Face", new AIProjectInfo()
            {
                Name = "人脸识别",
                Code = "Face",
                Remark = "人脸检测与识别是基于人工智能的生物识别技术，通过设备采集人脸图像，先检测定位人脸区域，再提取人脸特征并进行比对，实现快速确认人员身份、精准核验等功能。",
                ParamList = new List<AIProjectParam>()
                {
                    new AIProjectParam()
                    {
                        name="人脸阈值",
                        code="threshold",
                        type="float",
                        min=0,
                        max=1,
                        help="0~1的区间值,值越小对人脸的检测越模糊"
                    },
                    new AIProjectParam()
                    {
                        name="交并阈值",
                        code="iou_threshold",
                        type="float",
                        min=0,
                        max=1,
                        help="0~1的区间值,值越小越不会检测重合人脸"
                    },
                    new AIProjectParam()
                    {
                        name="启用人脸库",
                        code="enable_house",
                        type="boolean",
                        help="是否匹配人脸库，并触发相应事件"
                    },
                }
            });
        }
    }
}
