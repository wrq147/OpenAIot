using ChannelUtility;
using ChannelUtility.Message;
using ChannelUtility.Tsl;
using Common;
using Common.DataAc;
using Common.EventBus;
using EasyNetQ;
using IoTAIService.AICode;
using IoTAIService.Business;
using IoTAIService.DAL;
using IoTAIService.Models;
using IoTRulesService.DataParser;
using IoTService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Org.BouncyCastle.Asn1.Cms;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.NetCore;

namespace IoTAIService
{
    public class PluginConfig : TANetCorePluginConfig
    {
        public override string[] DependOn => new string[] { "IoTRulesService", "DeveloperService" };


        protected override void ConfigureServices(IConfiguration config, IServiceCollection services)
        {
            var cs = config.GetSection("IoTAIService");
            services.Configure<IoTAIOption>(cs);

            services.AddDAL<AiMemDAL>();
            services.AddDAL<AiHouseDAL>();
            services.AddSingleton<MilvusBLL>();
            services.AddBLL<AiMemBLL>();

            services.AddSingleton<FaceDetOnnxRunner>();
            services.AddSingleton<FaceRecogRunner>();
            services.AddSingleton<FaceSTNRunner>();
            services.AddSingleton<FaceKeyPointsRunner>();
            services.AddSingleton<AICache>();
        }
        private ITAServiceProvider _provider;
        protected override async void Configure(ITAApplication app, PluginObject plg)
        {
            _provider = app.ServiceProvider;
            TAEventDispatcher.Instance.RegisterPluginAllLoad(async (evt) =>
            {
                var aiOption = app.ServiceProvider.GetService<IOptions<IoTAIOption>>();
                if (aiOption.Value.InitMilvus == true)
                {
                    var milvusBLL = app.ServiceProvider.GetService<MilvusBLL>();
                    await milvusBLL.CreateMemberCollection();
                }

            });
            var redis = app.ServiceProvider.GetService<GeneralRedisHelper>();
            redis.HashSet("AI-Items", "Face", new AIProjectInfo()
            {
                Name = "人脸识别",
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
            app.ServiceProvider.GetService<MessageRunner>().OtherMessageListener += MessageHandler;

        }
        public override void Unload(ITAApplication app, PluginObject plg)
        {
            app.ServiceProvider.GetService<MessageRunner>().OtherMessageListener -= MessageHandler;
            base.Unload(app, plg);
        }
        private async Task DownAIDetectResponse(string nodeid, string videoId, string detType, List<BoxItem> boxlist)
        {
            AIDetectResponseMessage msg = new AIDetectResponseMessage();
            msg.DeviceId = videoId;
            msg.ProductId = string.Empty;
            var bus = _provider.GetService<RabbitScope>().Bus;
            string msgbody = System.Text.Json.JsonSerializer.Serialize(msg, JsonMessageSerializerConfig.DefaultOptions);
            await bus.PubSub.PublishAsync(msgbody, "/device." + nodeid + ".guid").ConfigureAwait(false);
        }
        private async Task MessageHandler(BaseDeviceMessage msg)
        {
            switch (msg.MsgType)
            {
                case "AIDetectReq":
                    {
                        AIDetectRequestMeesage detectReq = (AIDetectRequestMeesage)msg;
                        using (var ms = new MemoryStream(detectReq.RgbFrame))
                        using (var image = Image.Load<Rgb24>(ms))
                        {
                            switch (detectReq.DetType)
                            {
                                case "Face":
                                    {
                                        var tparam = new DataDetectParam(detectReq.DetParams);
                                        float tThreshold = tparam.GetFloat("threshold", 0.8f);
                                        float tIOU = tparam.GetFloat("iou_threshold", 0.2f);
                                        bool tEnableHouse = tparam.GetBool("enable_house");
                                        var aiCache = _provider.GetService<AICache>();
                                        var tbbx = _provider.GetService<FaceDetOnnxRunner>().Predict(image, tThreshold, tIOU);
                                        if (detectReq.IsDraw)
                                        {
                                            List<BoxItem> boxlist = new List<BoxItem>();
                                            for (int i = 0; i < tbbx.Count; i++)
                                            {
                                                var titem = tbbx[i];
                                                boxlist.Add(new BoxItem()
                                                {
                                                    x1 = titem.X1,
                                                    x2 = titem.X2,
                                                    y1 = titem.Y1,
                                                    y2 = titem.Y2,
                                                    score = titem.Score,
                                                    label = "人脸",
                                                    color = "#67C23A"
                                                });
                                            }
                                            await DownAIDetectResponse(detectReq.NodeId, detectReq.DeviceId, detectReq.DetType, boxlist);
                                        }

                                        //开始生成设备属性和事件
                                        int lastFaceNum = aiCache.GetVideoInt(detectReq.DeviceId, "face_num");
                                        if (lastFaceNum != tbbx.Count)
                                        {
                                            //发送人脸数量属性
                                            Dictionary<string, object> newvals = new Dictionary<string, object>();
                                            newvals.Add("FaceCount", tbbx.Count);
                                            await _provider.GetService<ServerBusProxy>().SendPropertyReply(string.Empty, detectReq.DeviceId, newvals);
                                            //人脸数量变化事件

                                        }
                                        aiCache.SetVideoInt(detectReq.DeviceId, "face_num", tbbx.Count);
                                    }
                                    break;
                            }
                        }
                    }
                    break;
            }
        }
    }
}
