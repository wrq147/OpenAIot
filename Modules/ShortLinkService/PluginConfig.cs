using Common;
using Common.DataAc;
using Common.EventBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ShortLinkService.Business;
using ShortLinkService.DAL;
using TemplateAction.Core;
using TemplateAction.NetCore;

namespace ShortLinkService
{
    public class PluginConfig : TANetCorePluginConfig
    {
        public override string[] DependOn => new string[] { "AuthService" };
        protected override void ConfigureServices(IConfiguration config, IServiceCollection services)
        {
            services.AddBLL<ShortLinkBLL>();
            services.AddDAL<ShortLinkDAL>();
        }
        protected override void Configure(ITAApplication app, PluginObject plg)
        {
            plg.RegisterCall("GetShortLink", async (evt) =>
            {
                var tOrgId = evt.GetLong("OrgId");
                var tUrl = evt.GetValue("Url");
                var tmpRes = await app.ServiceProvider.GetService<ShortLinkBLL>().Add(tUrl, tOrgId);
                return CallResponse.CreateFrom(tmpRes);
            });

        }
    }
}
