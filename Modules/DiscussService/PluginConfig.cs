using Common;
using DiscussService.Business;
using DiscussService.DAL;
using Microsoft.Extensions.Configuration;
using System;
using TemplateAction.Core;
using TemplateAction.NetCore;

namespace DiscussService
{
    public class PluginConfig : TANetCorePluginConfig
    {
        public override string[] DependOn => new string[] { "AuthService", "DictService" };
        protected override void ConfigureServices(IConfiguration config, IServiceCollection services)
        {

            var cs = config.GetSection("DiscussService");
            services.Configure<DiscussOption>(cs);
            services.AddBLL<CommentBLL>();
            services.AddDAL<CommentDAL>();
            services.AddDAL<SubjectDAL>();

        }
        protected override void Configure(ITAApplication app, PluginObject plg)
        {

        }
    }
}
