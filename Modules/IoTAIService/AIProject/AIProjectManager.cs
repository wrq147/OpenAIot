using ChannelUtility;
using ChannelUtility.Message;
using Common.EventBus;
using Common.Share;
using IoTAIService.AICode;
using IoTAIService.AIProject.Items;
using NATS.Client.Core;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Label;

namespace IoTAIService.AIProject
{
    public class AIProjectManager
    {
        private ITAServiceProvider _provider;
        private Dictionary<string, IDetect> _detects;
        private Dictionary<string, Infer> _infers;
        public AIProjectManager(ITAServiceProvider provider)
        {
            _provider = provider;
            _detects = new Dictionary<string, IDetect>();
            _infers = new Dictionary<string, Infer>();
        }
        public void RegDetect(string name, IDetect det)
        {
            _detects.Add(name, det);
        }
        public void RegInfer(string name, Infer infer)
        {
            _infers.Add(name, infer);
        }
        public async Task Init()
        {
            await new GeneralDetect().Init(_provider);
            await new FaceDetect().Init(_provider);
            await new FaceRecog().Init(_provider);
            await new GeneralTrigger().Init(_provider);
            await new PoseDetect().Init(_provider);
            await new BehaviorAnalysis().Init(_provider);
            await new ImageRecog().Init(_provider);
        }

        private async Task DownAIDetectResponse(string nodeid, string videoId, List<BoxItem> boxlist, bool needConf = false)
        {
            AIDetectResponseMessage msg = new AIDetectResponseMessage();
            msg.DeviceId = videoId;
            msg.ProductId = string.Empty;
            msg.NeedConf = needConf;
            msg.BoxList = boxlist;
            var bus = _provider.GetService<NatsScope>().Bus;
            string msgbody = System.Text.Json.JsonSerializer.Serialize(msg, JsonMessageSerializerConfig.DefaultOptions);

            await bus.PublishAsync(new NatsMsg<string>()
            {
                Subject = "node." + nodeid,
                Data = msgbody
            }, DefalutNatsJsonSerializer<string>.Default).ConfigureAwait(false);
        }

        public async Task<BusResponse<string>> TestDetect(string code, string img, string paramsJson)
        {
            var base64Str = img.Replace("data:image/png;base64,", "").Replace("data:image/jpg;base64,", "").Replace("data:image/jpeg;base64,", "");
            if (_detects.TryGetValue(code, out IDetect tmpdet))
            {
                string rsbase64 = null;
                byte[] byteArray = Convert.FromBase64String(base64Str);
                using (Image<Rgb24> rgbImage = Image.Load<Rgb24>(byteArray))
                {
                    var paramDict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(paramsJson, JsonMessageSerializerConfig.DefaultOptions);
                    AIConfigData config = new AIConfigData();
                    config.DetType = code;
                    config.DetParams = paramDict;
                    var boxs = tmpdet.GenerateBoxs(rgbImage, byteArray, config);
                    List<BoxItem> items = new List<BoxItem>();
                    foreach (var itembox in boxs)
                    {
                        items.Add(new BoxItem()
                        {
                            x1 = itembox.x1,
                            x2 = itembox.x2,
                            y1 = itembox.y1,
                            y2 = itembox.y2,
                            score = itembox.score,
                            label = itembox.label,
                            color = itembox.color
                        });
                    }

                    rsbase64 = AIUtility.DrawJpeg(rgbImage, items);
                }
                if (string.IsNullOrEmpty(rsbase64))
                {
                    return BusResponse<string>.Error(210, "返回错误");
                }
                return BusResponse<string>.Success(rsbase64);
            }
            else
            {
                return BusResponse<string>.Error(211, "AI项目不存在");
            }
        }
        public async Task MessageHandler(AIDetectRequestMeesage detectReq, List<AIConfigData> configs)
        {
            try
            {
                var aiCache = _provider.GetService<AICache>();
                if (detectReq.DataType == 0)
                {
                    //清除ai视频处理数据
                    aiCache.ClearVideo(detectReq.DeviceId);
                    return;
                }
                else if (detectReq.DataType == 1)
                {
                    List<AIConfigData> videoConfigs;
                    if (configs != null)
                    {
                        videoConfigs = configs;
                        aiCache.SetVideoAIConfig(detectReq.DeviceId, videoConfigs);
                    }
                    else
                    {
                        videoConfigs = aiCache.GetVideoAIConfig(detectReq.DeviceId);
                    }
                    if (videoConfigs == null)
                    {
                        await DownAIDetectResponse(detectReq.NodeId, detectReq.DeviceId, null, true);
                        return;
                    }
                    byte[] frameData = detectReq.Frame;
                    using (var ms = new MemoryStream(frameData))
                    {
                        var image = Image.Load<Rgb24>(ms);
                        if (image == null)
                        {
                            Console.WriteLine("AI解释异常：视频帧不存在");
                            return;
                        }
                        //处理画框
                        List<BoxItem> boxlist = new List<BoxItem>();
                        foreach (var config in videoConfigs)
                        {
                            if (_detects.TryGetValue(config.DetType, out IDetect tmpdet))
                            {
                                var boxs = tmpdet.GenerateBoxs(image, frameData, config);
                                boxlist.AddRange(boxs);
                            }
                        }
                        var videoData = aiCache.GetVideoCache(detectReq.DeviceId);
                        var totalBoxCount = videoData.GetInt("total_box_count");
                        bool needback = totalBoxCount != 0 || boxlist.Count != 0;
                        if (needback)
                        {
                            //回复画框
                            await DownAIDetectResponse(detectReq.NodeId, detectReq.DeviceId, boxlist);
                        }
                        videoData.SetInt("total_box_count", boxlist.Count);

                        // 更新跟踪器
                        videoData.UpdateByteTrack(boxlist);

                        //处理事件
                        if (needback)
                        {
                            foreach (var config in videoConfigs)
                            {
                                if (_infers.TryGetValue(config.DetType, out Infer tmpinfer))
                                {
                                    await tmpinfer.Execute(detectReq, image, config, boxlist);
                                }
                            }
                        }

                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }
    }
}
