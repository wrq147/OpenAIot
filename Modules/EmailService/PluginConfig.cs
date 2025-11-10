using Common;
using Common.EventBus;
using EmailService.Business;
using Microsoft.Extensions.Configuration;
using TemplateAction.Core;
using TemplateAction.NetCore;

namespace EmailService
{
    public class PluginConfig : TANetCorePluginConfig
    {
        public override string[] DependOn => new string[] { "AuthService" };
        protected override void ConfigureServices(IConfiguration config, IServiceCollection services)
        {
            services.AddBLL<EmailBLL>();
            services.AddSingleton<EmailExecutor>();
            //添加Email服务
            services.AddSingleton<EmailSenderHelper>();
        }
        protected override void Configure(ITAApplication app, PluginObject plg)
        {
            plg.RegisterNotice(async (evt) =>
            {
                await app.ServiceProvider.GetService<EmailExecutor>().SyncNoticeMessage(evt);
            });
        }
    }
}
