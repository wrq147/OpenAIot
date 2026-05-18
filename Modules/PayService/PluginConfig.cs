using Common;
using Microsoft.Extensions.Configuration;
using PayService.Business;
using PayService.DAL;
using System;
using TemplateAction.Core;
using TemplateAction.NetCore;

namespace PayService
{
    public class PluginConfig : TANetCorePluginConfig
    {
        public override string[] DependOn => new string[] { "AuthService" };
        protected override void ConfigureServices(IConfiguration config, IServiceCollection services)
        {
            services.AddBLL<PayChannelBLL>();
            services.AddBLL<PayDetailBLL>();
            services.AddBLL<WalletBLL>();

            services.AddDAL<PayChannelDAL>();
            services.AddDAL<PayDetailDAL>();
            services.AddDAL<WalletDAL>();
        }
        protected override void Configure(ITAApplication app, PluginObject plg)
        {
        }
    }
}
