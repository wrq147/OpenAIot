using ChannelUtility.Message;
using Common;
using IoTAIService.AICode;
using IoTAIService.Business;
using IoTAIService.DAL;
using IoTAIService.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
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
    /// 文字识别
    /// </summary>
    public class OcrRecog : Infer
    {
        private ITAServiceProvider _provider;

        public override async Task Execute(AIDetectRequestMeesage req, Image<Rgb24> image, AIConfigData config, List<BoxItem> boxes)
        {
          
        }

        public override async Task Init(ITAServiceProvider provider)
        {
            _provider = provider;
            string tkey = "OcrRecog";
            provider.GetService<AIProjectManager>().RegInfer(tkey, this);
            var redis = provider.GetService<GeneralRedisHelper>();
            await redis.HashSetAsync("AI-Items", tkey, new AIProjectInfo()
            {
                Name = "文字识别",
                Code = tkey,
                Stage = "Infer",
                Remark = "文本识别模块是OCR系统中的核心部分，负责从图像中的文本区域提取出文本信息",
                ParamList = new List<AIProjectParam>()
            });
        }
    }
}
