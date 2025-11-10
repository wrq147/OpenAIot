using System;
using TemplateAction.Core;
using TemplateAction.NetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using DeveloperService;
using DeveloperService.DAL;
using Common;
using DeveloperService.Business;
using Common.EventBus;
using Common.Share;
using TemplateAction.Common;
using DeveloperService.Model;

namespace DeveloperService
{
    public class PluginConfig : TANetCorePluginConfig
    {
        public override string[] DependOn => new string[] { "AuthService" };
        protected override void ConfigureServices(IConfiguration config, IServiceCollection services)
        {
            services.AddSingleton<DeveloperMiddleware>();
            services.AddBLL<DeveloperBLL>();
            services.AddDAL<DeveloperDAL>();

            services.Configure<DeveloperOption>(config.GetSection("FlowService"));
        }
        protected override void Configure(ITAApplication app, PluginObject plg)
        {
            //使用身份认证
            ((TASiteApplication)app).UseMiddlewareFirst<DeveloperMiddleware>();

            plg.RegisterBus("ChangeCreator", async (bs) =>
            {
                await app.ServiceProvider.GetService<DeveloperBLL>().ChangeOrgUser(bs.GetLong("id"), bs.GetLong("uid"));
            });

        }


    }
}
