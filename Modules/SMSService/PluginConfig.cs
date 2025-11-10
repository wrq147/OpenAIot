using AuthService;
using Common.EventBus;
using Microsoft.Extensions.Configuration;
using SMSService.Business;
using System;
using TemplateAction.Core;
using TemplateAction.NetCore;

namespace SMSService
{
    public class PluginConfig : TANetCorePluginConfig
    {
        protected override void ConfigureServices(IConfiguration config, IServiceCollection services)
        {
            services.AddScope<SmsCodeBLL>();
            services.AddSingleton<SMSExecutor>();
            services.AddSingleton<AliSmsHelper>();
            services.AddSingleton<JsonSmsHelper>();
            services.AddSingleton<ShanYunSmsHelper>();
        }
        protected override void Configure(ITAApplication app, PluginObject plg)
        {
            plg.RegisterNotice(async (evt) =>
            {
                await app.ServiceProvider.GetService<SMSExecutor>().SyncNoticeMessage(evt);
            });


        }
    }
}
