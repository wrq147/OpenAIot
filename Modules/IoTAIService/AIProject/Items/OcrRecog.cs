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
    /// 文字识别
    /// </summary>
    public class OcrRecog : Infer
    {
        private ITAServiceProvider _provider;

        public override async Task Execute(AIDetectRequestMeesage req, Image<Rgb24> image, AIConfigData config, List<BoxItem> boxes)
        {
            var aiCache = _provider.GetService<AICache>();
            var videoData = aiCache.GetVideoCache(req.DeviceId);
            var boxlist = boxes.Where(x => x.label == "文本").ToList();
            var ocrRecRunner = _provider.GetService<OcrRecRunner>();
            if (boxlist.Count > 0)
            {
                string oldtxt = videoData.GetItem<string>("ocrwords");

                List<string> tmplist = new List<string>();
                foreach (var titem in boxlist)
                {
                    var tmpimg = image.CropByBox(titem.x1, titem.x2, titem.y1, titem.y2);
                    var tmpstr = ocrRecRunner.Predict(tmpimg);
                    tmplist.Add(tmpstr);
                }
                string allstrs = string.Join(",", tmplist);
                if (oldtxt != allstrs)
                {
                    var aiBusProxy = _provider.GetService<AIBusProxy>();
                    Dictionary<string, object> newvals = new Dictionary<string, object>();
                    newvals.Add("OcrText", allstrs);
                    await aiBusProxy.SendPropertyReply(string.Empty, req.DeviceId, newvals);
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
