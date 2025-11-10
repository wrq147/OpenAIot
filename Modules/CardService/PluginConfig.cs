using AuthService;
using CardService.Business;
using CardService.DAL;
using Common;
using Microsoft.Extensions.Configuration;
using System;
using TemplateAction.Core;
using TemplateAction.NetCore;
using WeiXinService.Business;

namespace CardService
{
    public class PluginConfig : TANetCorePluginConfig
    {
        public override string[] DependOn => new string[] { "DictService", "WeiXinService" };
        protected override void ConfigureServices(IConfiguration config, IServiceCollection services)
        {
            services.AddBLL<CardSmsBLL>();
            services.AddBLL<CardHolderBLL>();
            services.AddBLL<CardManBLL>();
            services.AddBLL<CardProBLL>();
            services.AddBLL<CardCaseBLL>();
            services.AddBLL<CardMsgBLL>();
            services.AddBLL<CardBLL>();
            services.AddBLL<CardExchangeBLL>();

            services.AddBLL<CardOrgBLL>();


            services.AddDAL<CardExchangeDAL>();
            services.AddDAL<CardHolderDAL>();
            services.AddDAL<CardOrgDAL>();
            services.AddDAL<CardAdminDAL>();
            services.AddDAL<CardProDAL>();
            services.AddDAL<CardCaseDAL>();
            services.AddDAL<CardMsgDAL>();
            services.AddDAL<CardDAL>();

        }
        protected override void Configure(ITAApplication app, PluginObject plg)
        {
        }

    }
}
