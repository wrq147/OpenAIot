using System;
using System.Collections.Generic;
using Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace IoTAIService.AIProject.Items
{
    /// <summary>
    /// 通用检测项目
    /// </summary>
    public static class DetectionProject
    {
        public static async Task Init(ITAServiceProvider provider)
        {
            var redis = provider.GetService<GeneralRedisHelper>();
            await redis.HashSetAsync("AI-Items", "Detect", new AIProjectInfo()
            {
                Name = "通用检测",
                Code = "Detect",
                Remark = "通用检测能够根据描述性文本检测图像中的任何物体。",
                ParamList = new List<AIProjectParam>()
                {
                    new AIProjectParam()
                    {
                        name="目标特征",
                        code="feature",
                        type="clip",
                        help="请预先生成目标检测特征"
                    }
                }
            });
        }
    }
}
