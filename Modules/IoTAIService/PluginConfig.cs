using ChannelUtility.Message;
using Common;
using EasyNetQ.Consumer;
using IoTAIService.AICode;
using IoTAIService.Business;
using IoTAIService.DAL;
using IoTRulesService.DataParser;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Org.BouncyCastle.Asn1.Cms;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
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
        }
        protected override async void Configure(ITAApplication app, PluginObject plg)
        {
            TAEventDispatcher.Instance.RegisterPluginAllLoad(async (evt) =>
            {
                var aiOption = app.ServiceProvider.GetService<IOptions<IoTAIOption>>();
                if (aiOption.Value.InitMilvus == true)
                {
                    var milvusBLL = app.ServiceProvider.GetService<MilvusBLL>();
                    await milvusBLL.CreateMemberCollection();
                }

            });

            app.ServiceProvider.GetService<MessageRunner>().OtherMessageListener += MessageHandler;

        }
        public override void Unload(ITAApplication app, PluginObject plg)
        {
            app.ServiceProvider.GetService<MessageRunner>().OtherMessageListener -= MessageHandler;
            base.Unload(app, plg);
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

                        }
                    }
                    break;
            }
        }
    }
}
