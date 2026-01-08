using ChannelUtility;
using ChannelUtility.Message;
using ChannelUtility.Tsl;
using Common;
using Common.DataAc;
using Common.EventBus;
using EasyNetQ;
using IoTAIService.AICode;
using IoTAIService.AIProject;
using IoTAIService.Business;
using IoTAIService.DAL;
using IoTAIService.Models;
using IoTRulesService.DataParser;
using IoTService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Org.BouncyCastle.Asn1.Cms;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.ColorSpaces;
using SixLabors.ImageSharp.PixelFormats;
using System;
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
            services.AddSingleton<AIProjectManager>();
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

                //初始化AI项目
                await app.ServiceProvider.GetService<AIProjectManager>().Init();
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
            await bus.PubSub.PublishAsync(msgbody, "/node." + nodeid).ConfigureAwait(false);
        }

        private Image<Rgb24> FastZlibDecompressToRgb24Image(byte[] compressedData, int width, int height)
        {
            // 入参校验
            if (compressedData == null || compressedData.Length == 0)
            {
                Console.WriteLine("压缩数据为空，解压失败");
                return null;
            }
            if (width <= 0 || height <= 0)
            {
                Console.WriteLine("宽高参数非法");
                return null;
            }

            try
            {
                int expectedLength = width * height * 3;

                // 步骤2：Zlib解压得到RGB24原始数据
                using (var msIn = new MemoryStream(compressedData))
                using (var zlibStream = new DeflateStream(msIn, CompressionMode.Decompress))
                using (var msOut = new MemoryStream(expectedLength))
                {
                    zlibStream.CopyTo(msOut);
                    Image<Rgb24> rgbImage = Image.Load<Rgb24>(msOut);
                    return rgbImage;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"解压并创建Image失败：{ex.Message}");
                return null;
            }
        }

        private async Task MessageHandler(BaseDeviceMessage msg)
        {
            switch (msg.MsgType)
            {
                case "AIDetectReq":
                    {
                        AIDetectRequestMeesage detectReq = (AIDetectRequestMeesage)msg;

                        using (var image = FastZlibDecompressToRgb24Image(detectReq.Frame, detectReq.Width, detectReq.Height))
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
