

using System;

namespace TemplateAction.Core
{
    /// <summary>
    /// 空配置文件
    /// </summary>
    public class EmptyConfig : IPluginConfig
    {
        public string[] DependOn => Array.Empty<string>();
        public void Loaded(ITAApplication app, IServiceCollection services, PluginObject plg)
        {
        }

        public void Unload(ITAApplication app, PluginObject plg)
        {
        }
    }
}
