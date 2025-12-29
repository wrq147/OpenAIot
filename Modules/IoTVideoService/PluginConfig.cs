using ChannelUtility.Message;
using Common;
using Common.EventBus;
using IoTRulesService.DataParser;
using IoTService;
using IoTVideoService.Business;
using IoTVideoService.DAL;
using Microsoft.Extensions.Configuration;
using MonitorService.Business;
using MonitorService.Model;
using System;
using TemplateAction.Core;
using TemplateAction.NetCore;
namespace IoTVideoService
{
    public class PluginConfig : TANetCorePluginConfig
    {
        public override string[] DependOn => new string[] { "IoTService", "IoTRulesService" };


        protected override void ConfigureServices(IConfiguration config, IServiceCollection services)
        {
            services.AddBLL<VideoSourceBLL>();
            services.AddDAL<VideoSourceDAL>();
            services.Configure<VideoOption>(config.GetSection("IoTVideoService"));
        }
        private ITAServiceProvider _provider;
        protected override void Configure(ITAApplication app, PluginObject plg)
        {
            _provider = app.ServiceProvider;


            app.ServiceProvider.GetService<MessageRunner>().OtherMessageListener += MessageHandler;
        }

        private async Task MessageHandler(BaseDeviceMessage msg)
        {
            switch (msg.MsgType)
            {
                case "MediaNF":
                    MediaNotFoundMessage nfmsg = (MediaNotFoundMessage)msg;
                    await _provider.GetService<VideoSourceBLL>().CollectVideo(nfmsg);
                    break;
                case "MediaNR":
                    MediaNotReaderMessage nrmsg = (MediaNotReaderMessage)msg;
                    await _provider.GetService<VideoSourceBLL>().DelVideo(nrmsg);
                    break;
            }
        }
    }
}
