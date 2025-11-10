using Microsoft.Extensions.Configuration;
using TemplateAction.Core;
using TemplateAction.NetCore;

namespace ThirdPartyService
{
    public class PluginConfig : TANetCorePluginConfig
    {
        protected override void ConfigureServices(IConfiguration config, IServiceCollection services)
        {
        }
        protected override void Configure(ITAApplication app, PluginObject plg)
        {
        }
    }
}
