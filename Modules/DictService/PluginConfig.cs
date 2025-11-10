using DictService.Business;
using DictService.DAL;
using System;
using TemplateAction.Core;
using Common;
using TemplateAction.NetCore;
using Microsoft.Extensions.Configuration;

namespace DictService
{
    public class PluginConfig : TANetCorePluginConfig
    {
        public override string[] DependOn => new string[] { "AuthService" };
        protected override void ConfigureServices(IConfiguration config, IServiceCollection services)
        {
            services.AddBLL<DictDataBLL>();
            services.AddBLL<DictTypeBLL>();
            services.AddDAL<DictDataDAL>();
            services.AddDAL<DictTypeDAL>();
        }
        protected override void Configure(ITAApplication app, PluginObject plg)
        {

        }

    }
}
