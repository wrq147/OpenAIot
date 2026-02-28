using ChannelUtility;
using ChannelUtility.Message;
using Common.EventBus;
using IoTAIService.AICode;
using IoTAIService.AIProject.Items;
using NATS.Client.Core;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace IoTAIService.AIProject
{
    public class AIProjectManager
    {
        private ITAServiceProvider _provider;
        private Dictionary<string, IDetect> _detects;
        private Dictionary<string, IInfer> _infers;
        public AIProjectManager(ITAServiceProvider provider)
        {
            _provider = provider;
            _detects = new Dictionary<string, IDetect>();
            _infers = new Dictionary<string, IInfer>();
        }
        public void RegDetect(string name, IDetect det)
        {
            _detects.Add(name, det);
        }
        public void RegInfer(string name, IInfer infer)
        {
            _infers.Add(name, infer);
        }
        public async Task Init()
        {
            await new GeneralDetect().Init(_provider);
            await new FaceDetect().Init(_provider);
            await new FaceRecog().Init(_provider);
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
        public async Task MessageHandler(AIDetectRequestMeesage detectReq, List<AIConfigData> configs)
        {
            var aiCache = _provider.GetService<AICache>();
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
            using (var image = FastZlibDecompressToRgb24Image(frameData, detectReq.Width, detectReq.Height))
            {
                //处理画框
                List<BoxItem> boxlist = new List<BoxItem>();
                foreach (var config in videoConfigs)
                {
                    if (_detects.TryGetValue(config.DetType, out IDetect tmpdet))
                    {
                        var boxs = tmpdet.GenerateBoxs(detectReq.DeviceId, image, config);
                        boxlist.AddRange(boxs);
                    }
                }

                //回复画框
                await DownAIDetectResponse(detectReq.NodeId, detectReq.DeviceId, boxlist);


                //处理事件
                foreach (var config in videoConfigs)
                {
                    if (_infers.TryGetValue(config.DetType, out IInfer tmpinfer))
                    {
                        await tmpinfer.Execute(detectReq.DeviceId, image, config, boxlist);
                    }
                }

            }
        }
    }
}
