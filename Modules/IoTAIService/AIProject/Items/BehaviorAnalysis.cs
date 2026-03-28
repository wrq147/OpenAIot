using ChannelUtility.Message;
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
using Common;
namespace IoTAIService.AIProject.Items
{
    /// <summary>
    /// 行为分析
    /// </summary>
    public class BehaviorAnalysis : IInfer
    {
        private ITAServiceProvider _provider;
        public async Task Init(ITAServiceProvider provider)
        {
            _provider = provider;
            string tkey = "Behavior";
            provider.GetService<AIProjectManager>().RegInfer(tkey, this);
            var redis = provider.GetService<GeneralRedisHelper>();
            Dictionary<string, string> tmpbetypes = new Dictionary<string, string>();
            for (int i = 0; i < PoseC3DRunner.NTU120_Actions_Chinese.Length; i++)
            {
                tmpbetypes.Add(PoseC3DRunner.NTU120_Actions_Chinese[i], i.ToString());
            }
            await redis.HashSetAsync("AI-Items", tkey, new AIProjectInfo()
            {
                Name = "行为分析",
                Code = tkey,
                Stage = "Infer",
                Remark = "人体行为分析是在人体检测、目标跟踪、姿态识别基础上，对视频画面中人员的动作、轨迹、状态、交互关系进行智能理解与异常判断。",
                ParamList = new List<AIProjectParam>()
                {
                    new AIProjectParam()
                    {
                        name="告警行为",
                        code="be_type",
                        type="enum",
                        elements=tmpbetypes,
                        multi=true,
                        defval=Array.Empty<string>(),
                        help="限制触发事件的行为"
                    },
                    new AIProjectParam()
                    {
                        name="行为阈值",
                        code="threshold",
                        type="float",
                        defval=0.8f,
                        min=0,
                        max=1,
                        help="0~1的区间值,值越小,对行为的判断越模糊"
                    },
                }
            });
        }

        public async Task Execute(AIDetectRequestMeesage req, Image<Rgb24> image, AIConfigData config, List<BoxItem> boxes)
        {
            float tThreshold = config.GetFloat("threshold", 0.8f);
            List<object> tLimit = config.Get<List<object>>("be_type");
            var aiCache = _provider.GetService<AICache>();
            var videoData = aiCache.GetVideoCache(req.DeviceId);
            var aiBusProxy = _provider.GetService<AIBusProxy>();

            var tracklist = videoData.TrackList;
            var addlist = videoData.AddTrackList;
            //姿势识别
            Dictionary<int, List<BoxItem>> trackHistorys = videoData.GetItem<Dictionary<int, List<BoxItem>>>("track_his");
            if (trackHistorys == null)
            {
                trackHistorys = new Dictionary<int, List<BoxItem>>();
                videoData.SetItem("track_his", trackHistorys);
            }
            //删除无用历史
            var tkeys = trackHistorys.Keys;
            foreach (var hisKey in tkeys)
            {
                if (!tracklist.Exists(x => x.Id == hisKey))
                {
                    trackHistorys.Remove(hisKey);
                }
            }
            var posec3d = _provider.GetService<PoseC3DRunner>();
            foreach (var trackItem in tracklist)
            {
                List<BoxItem> tmpboxlist;
                if (!trackHistorys.TryGetValue(trackItem.Id, out tmpboxlist))
                {
                    tmpboxlist = new List<BoxItem>();
                    trackHistorys[trackItem.Id] = tmpboxlist;
                }
                tmpboxlist.Add(trackItem.CurrentDetection);
                if (tmpboxlist.Count >= 48)
                {
                    (int classId, string className, float score) = posec3d.Process(tmpboxlist);
                    if (score >= tThreshold)
                    {
                        bool cansend = false;
                        if (tLimit != null)
                        {
                            foreach (var tlimit in tLimit)
                            {
                                if (classId == (int)tlimit)
                                {
                                    cansend = true;
                                }
                            }
                        }
                        else
                        {
                            cansend = true;
                        }
                        if (cansend)
                        {
                            Dictionary<string, object> outputs = new Dictionary<string, object>();
                            outputs.Add("ac_name", className);
                            await aiBusProxy.SendEvent(string.Empty, req.DeviceId, "Behavior", outputs);
                        }

                    }
                    tmpboxlist.RemoveRange(0, 8);
                }
            }

        }

    }
}
