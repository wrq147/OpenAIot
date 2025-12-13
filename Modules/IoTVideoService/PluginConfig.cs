using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using TemplateAction.Core;
using TemplateAction.NetCore;

namespace IoTVideoService
{
    public class PluginConfig : TANetCorePluginConfig
    {
        public override string[] DependOn => new string[] { "IoTService" };

        protected override void Configure(ITAApplication app, PluginObject plg)
        {
        }

        protected override void ConfigureServices(IConfiguration config, IServiceCollection services)
        {
        }
    }
}
