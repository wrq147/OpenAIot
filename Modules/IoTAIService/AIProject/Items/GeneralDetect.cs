using System;
using System.Collections.Generic;
using Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;
using ChannelUtility.Message;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace IoTAIService.AIProject.Items
{
    /// <summary>
    /// 通用检测项目
    /// </summary>
    public class GeneralDetect : IDetect
    {
        private ITAServiceProvider _provider;
        public async Task Init(ITAServiceProvider provider)
        {
            _provider = provider;
            string tkey = "Detect";
            provider.GetService<AIProjectManager>().RegDetect(tkey, this);
            var redis = provider.GetService<GeneralRedisHelper>();
            await redis.HashSetAsync("AI-Items", tkey, new AIProjectInfo()
            {
                Name = "通用检测",
                Code = tkey,
                Stage= "Detect",
                Remark = "通用检测能够根据描述性文本检测图像中的任何物体。",
                ParamList = new List<AIProjectParam>()
                {
                    new AIProjectParam()
                    {
                        name="目标特征",
                        code="feature",
                        type="clip",
                        help="请预先生成目标检测特征"
                    },
                    new AIProjectParam() {
                        name="是否绘制检测框",
                        code="if_draw",
                        type="boolean",
                        help="是否在视频上绘制检测框"
                    }
                }
            });
        }

        public List<BoxItem> GenerateBoxs(Image<Rgb24> image, AIConfigData config)
        {
            throw new NotImplementedException();
        }

    }
}
