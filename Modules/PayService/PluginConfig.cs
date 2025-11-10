using Microsoft.Extensions.Configuration;
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
        }
        protected override void Configure(ITAApplication app, PluginObject plg)
        {
        }
    }
}
