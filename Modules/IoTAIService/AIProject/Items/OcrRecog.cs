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
            var aiCache = _provider.GetService<AICache>();
            var videoData = aiCache.GetVideoCache(req.DeviceId);
            var tracklist = videoData.TrackList.Where(x => x.CurrentDetection.label == "文本").ToList();
            var addlist = videoData.AddTrackList.Where(x => x.CurrentDetection.label == "文本").ToList();
            var ocrRecRunner = _provider.GetService<OcrRecRunner>();
            if (addlist.Count > 0)
            {
                var addDetects = addlist.Select(x => x.CurrentDetection);
                foreach (var titem in addDetects)
                {
                    var tmpimg = image.CropByBox(titem.x1, titem.x2, titem.y1, titem.y2);

                }
            }
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
