using System;

namespace TemplateAction.Core
{
    /// <summary>
    /// 插件加载器工厂
    /// </summary>
    public interface ILoaderFactory
    {
        IPluginLoader CreateLoader(PluginCollection collection);
    }
}
