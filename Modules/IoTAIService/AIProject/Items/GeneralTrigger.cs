using ChannelUtility.Message;
using Common;
using IoTAIService.AICode;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Dynamic;
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
            var stayTime = config.GetFloat("stay_time", 5);
            var directList = config.Get<List<object>>("direct");
            var directStrList = directList.Select(x => (string)x).ToList();

            #region 通用物体跟踪
            var tracker = videoData.GetItem<ByteTrack>("gen_track");
            if (tracker == null)
            {
                tracker = new ByteTrack(trackThresh: 0.5f, trackLowThresh: 0.1f, matchThresh: 0.8f);
            }
            (var tracklist, var addlist) = tracker.Update(boxes);
            #endregion

            var regionList = videoData.GetItem<List<MonitoringRegion>>("gen_region");
            if (regionList == null)
            {
                regionList = new List<MonitoringRegion>();
                var zonelist = config.Get<List<object>>("inv_zone");
                if (zonelist != null && zonelist.Count > 0)
                {
                    int i = 1;
                    foreach (var tmpzone in zonelist)
                    {
                        dynamic tmpobj = tmpzone as ExpandoObject;
                        if (tmpobj != null)
                        {
                            if (tmpobj.type == "Rectangle")
                            {
                                var tmppoints = tmpobj.points as List<object>;
                                dynamic startPoint = tmppoints[0] as ExpandoObject;
                                dynamic endPoint = tmppoints[1] as ExpandoObject;
                                regionList.Add(MonitoringRegion.CreateRectangle(startPoint.x, startPoint.y, endPoint.x, endPoint.y, string.Empty, $"矩形区域{i}"));
                            }
                            else if (tmpobj.type == "Polygon")
                            {
                                var tmppoints = tmpobj.points as List<object>;
                                var vectPoints = new List<System.Numerics.Vector2>();
                                foreach (var tmppoint in tmppoints)
                                {
                                    dynamic tmppp = tmppoint as ExpandoObject;
                                    vectPoints.Add(new System.Numerics.Vector2() { X = tmppp.x, Y = tmppp.y });
                                }
                                regionList.Add(MonitoringRegion.CreatePolygon(vectPoints, string.Empty, $"多边形区域{i}"));
                            }

                        }
                        ++i;
                    }
                }
                videoData.SetItem("gen_region", regionList);
            }

            if (regionList.Count > 0)
            {
                //区域入侵检测
                foreach (var trackItem in tracklist)
                {
                    foreach (var tmpregion in regionList)
                    {
                        var status = trackItem.UpdateRegionStatus(tmpregion);
                        if (status == RegionStatus.Inside || status == RegionStatus.Entered)
                        {
                            var stayInfo = trackItem.GetRegionStay(tmpregion.Id);
                            bool rightDir = true;
                            if (directStrList.Count > 0)
                            {
                                var curDir = trackItem.GetMovementDirection().ToString();
                                rightDir = directStrList.Contains(curDir);
                            }
                            if (rightDir && stayInfo != null && !stayInfo.IsSend && (DateTime.Now - stayInfo.EnterTime.Value).TotalSeconds > stayTime)
                            {
                                var tmpimg = image.CropByBox(trackItem.CurrentDetection.x1, trackItem.CurrentDetection.x2, trackItem.CurrentDetection.y1, trackItem.CurrentDetection.y2);
                                Dictionary<string, object> outputs = new Dictionary<string, object>();
                                outputs.Add("item_img", tmpimg.ToBase64String(JpegFormat.Instance));
                                await aiBusProxy.SendEvent(string.Empty, req.DeviceId, "ItemIn", outputs);
                                stayInfo.IsSend = true;
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                var adddets = addlist.Select(x => x.CurrentDetection);
                foreach (var track in tracklist)
                {
                    bool rightDir = true;
                    if (directStrList.Count > 0)
                    {
                        var curDir = track.GetMovementDirection().ToString();
                        rightDir = directStrList.Contains(curDir);
                    }
                    if (rightDir && !track.IsSend && (DateTime.Now - track.CreatedOn).TotalSeconds > stayTime)
                    {
                        var tmpimg = image.CropByBox(track.CurrentDetection.x1, track.CurrentDetection.x2, track.CurrentDetection.y1, track.CurrentDetection.y2);
                        Dictionary<string, object> outputs = new Dictionary<string, object>();
                        outputs.Add("item_img", tmpimg.ToBase64String(JpegFormat.Instance));
                        await aiBusProxy.SendEvent(string.Empty, req.DeviceId, "ItemIn", outputs);
                        track.IsSend = true;
                    }
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
                        name="停留时长（秒）",
                        code="stay_time",
                        type="float",
                        defval=5,
                        min=0,
                        max=3600,
                        help="设置触发的停留时长"
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
