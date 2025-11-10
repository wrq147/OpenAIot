using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TemplateAction.Core;
using TemplateAction.NetCore;

namespace SaleService
{
    public class PluginConfig : TANetCorePluginConfig
    {
        public override string[] DependOn => new string[] { "AuthService" };
        protected override void ConfigureServices(IConfiguration config, IServiceCollection services)
        {

        }
        protected override void Configure(ITAApplication app, PluginObject plg)
        {
        }
    }
}
