using ChannelUtility;
using ChannelUtility.Message;
using Common;
using Common.EventBus;
using IoTAIService.AICode;
using IoTAIService.AIProject;
using IoTAIService.Business;
using IoTAIService.DAL;
using IoTRulesService.DataParser;
using IoTService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NATS.Client.Core;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Security.Policy;
using System.Text;
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
            services.AddSingleton<PythonExe>();
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
        private async Task DownAIDetectResponse(string nodeid, string videoId, List<BoxItem> boxlist, bool needConf = false)
        {
            AIDetectResponseMessage msg = new AIDetectResponseMessage();
            msg.DeviceId = videoId;
            msg.ProductId = string.Empty;
            msg.NeedConf = needConf;
            var bus = _provider.GetService<NatsScope>().Bus;
            string msgbody = System.Text.Json.JsonSerializer.Serialize(msg, JsonMessageSerializerConfig.DefaultOptions);

            await bus.PublishAsync(new NatsMsg<string>()
            {
                Subject = "node." + nodeid,
                Data = msgbody
            }, DefalutNatsJsonSerializer<string>.Default).ConfigureAwait(false);
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
            if (msg.MsgType != "AIDetectReq")
            {
                return;
            }
            var aiCache = _provider.GetService<AICache>();
            AIDetectRequestMeesage detectReq = (AIDetectRequestMeesage)msg;
            List<AIConfigData> videoConfigs;
            if (detectReq.Configs != null)
            {
                videoConfigs = detectReq.Configs;
                aiCache.SetVideoAIConfig(detectReq.DeviceId, videoConfigs);
            }
            else
            {
                videoConfigs = aiCache.GetVideoAIConfig(detectReq.DeviceId);
            }
            if (videoConfigs == null)
            {
                await DownAIDetectResponse(detectReq.NodeGuid, detectReq.DeviceId, null, true);
                return;
            }
            byte[] frameData = Encoding.UTF8.GetBytes(detectReq.Frame);
            using (var image = FastZlibDecompressToRgb24Image(frameData, detectReq.Width, detectReq.Height))
            {
                int facenum = 0;
                //处理画框
                List<BoxItem> boxlist = new List<BoxItem>();
                foreach (var config in videoConfigs)
                {
                    switch (config.DetType)
                    {
                        case "Face":
                            {
                                var tparam = new DataDetectParam(config.DetParams);
                                float tThreshold = tparam.GetFloat("threshold", 0.8f);
                                float tIOU = tparam.GetFloat("iou_threshold", 0.2f);
                                bool tEnableHouse = tparam.GetBool("enable_house");

                                var tbbx = _provider.GetService<FaceDetOnnxRunner>().Predict(image, tThreshold, tIOU);
                                facenum = tbbx.Count;
                                if (config.IsDraw)
                                {
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
                                }
                            }
                            break;
                    }
                }
                //回复画框
                await DownAIDetectResponse(detectReq.NodeGuid, detectReq.DeviceId, boxlist);


                //处理事件
                foreach (var config in videoConfigs)
                {
                    switch (config.DetType)
                    {
                        case "Face":
                            {
                                //开始生成设备属性和事件
                                int lastFaceNum = aiCache.GetVideoInt(detectReq.DeviceId, "face_num");
                                if (lastFaceNum != facenum)
                                {
                                    //发送人脸数量属性
                                    Dictionary<string, object> newvals = new Dictionary<string, object>();
                                    newvals.Add("FaceCount", facenum);
                                    ReadPropertyMessageReply rpmsg = new ReadPropertyMessageReply();
                                    rpmsg.ProductId = string.Empty;
                                    rpmsg.DeviceId = detectReq.DeviceId;
                                    rpmsg.Timestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
                                    rpmsg.Properties = newvals;
                                    rpmsg.IsTagSync = false;
                                    await _provider.GetService<DeviceMessageHandler>().ExeMessage(rpmsg);
                                    //人脸数量变化事件

                                }
                                aiCache.SetVideoInt(detectReq.DeviceId, "face_num", facenum);
                            }
                            break;
                    }
                }
            }
        }
    }
}
