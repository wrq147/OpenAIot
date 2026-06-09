using Microsoft.Extensions.Configuration;
using System;
using TemplateAction.Core;

namespace TemplateAction.NetCore
{
    /// <summary>
    /// 抽像插件配置
    /// </summary>
    public abstract class TANetCorePluginConfig : IPluginConfig
    {
        /// <summary>
        /// 默认自动生成依赖
        /// </summary>
        public virtual string[] DependOn => null;

        public void Loaded(ITAApplication app, IServiceCollection services, PluginObject plg)
        {
            IConfiguration config = app.ServiceProvider.GetService<IConfiguration>();
            ConfigureServices(config, services);
            app.UsePlugin(plg);
            if (plg.IsService)
            {
                Configure(app, plg);
            }
        }
        /// <summary>
        /// 配置服务（插件激活前）
        /// </summary>
        /// <param name="config"></param>
        /// <param name="services"></param>
        protected abstract void ConfigureServices(IConfiguration config, IServiceCollection services);
        /// <summary>
        /// 服务初始化，非服务不会执行（插件激活后）
        /// </summary>
        /// <param name="app"></param>
        /// <param name="plg"></param>
        protected abstract void Configure(ITAApplication app, PluginObject plg);
        public virtual void Unload(ITAApplication app, PluginObject plg) { }
    }
}
