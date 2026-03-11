using ChannelUtility.Message;
using Common;
using IoTAIService.AICode;
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
    /// 通用推理项目
    /// </summary>
    public class GeneralTrigger : IInfer
    {
        private ITAServiceProvider _provider;
        public async Task Execute(AIDetectRequestMeesage req, Image<Rgb24> image, AIConfigData config, List<BoxItem> boxes)
        {
            var aiCache = _provider.GetService<AICache>();
            var videoData = aiCache.GetVideoCache(req.DeviceId);
            var aiBusProxy = _provider.GetService<AIBusProxy>();

            #region 通用物体跟踪
            var tracker = videoData.GetItem<ByteTrack>("gen_track");
            if (tracker == null)
            {
                tracker = new ByteTrack(trackThresh: 0.5f, trackLowThresh: 0.1f, matchThresh: 0.8f);
            }
            (var tracklist, var addlist, var rmlist) = tracker.Update(boxes);
            #endregion

            var zonelist = config.Get<List<object>>("inv_zone");
            if (zonelist != null && zonelist.Count > 0)
            {

            }

            if (addlist.Count > 0)
            {
                var adddets = addlist.Select(x => x.CurrentDetection);
                foreach (var titem in adddets)
                {
                    var tmpimg = image.CropByBox(titem.x1, titem.x2, titem.y1, titem.y2);

                    Dictionary<string, object> outputs = new Dictionary<string, object>();
                    outputs.Add("item_img", tmpimg.ToBase64String(JpegFormat.Instance));
                    await aiBusProxy.SendEvent(string.Empty, req.DeviceId, "ItemIn", outputs);
                }
            }
        }

        public async Task Init(ITAServiceProvider provider)
        {
            _provider = provider;
            string tkey = "GeneralTrigger";
            provider.GetService<AIProjectManager>().RegInfer(tkey, this);
            var redis = provider.GetService<GeneralRedisHelper>();
            await redis.HashSetAsync("AI-Items", tkey, new AIProjectInfo()
            {
                Name = "通用推理",
                Code = tkey,
                Stage = "Infer",
                Remark = "通过物体跟踪，触发物体入侵事件。",
                ParamList = new List<AIProjectParam>()
                {
                    new AIProjectParam()
                    {
                        name="入侵区域",
                        code="inv_zone",
                        type="region",
                        help="设置触发的检测区域"
                    },
                    new AIProjectParam()
                    {
                        name="移动方向",
                        code="direct",
                        type="enum",
                        multi=true,
                        elements=new Dictionary<string, string>()
                        {
                            { "未知", "Unknown" },
                            { "上", "Up" },
                            { "下", "Down" },
                            { "左", "Left" },
                            { "右", "Right" },
                            { "左上", "UpLeft" },
                            { "右上", "UpRight" },
                            { "左下", "DownLeft" },
                            { "右下", "DownRight" }
                        },
                        help="设置触发的移动方向"
                    },
                }
            });
        }
    }
}
