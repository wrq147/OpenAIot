namespace TemplateAction.Core
{
    /// <summary>
    /// 插件配置接口
    /// 每个插件加载时，会先执行此配置
    /// </summary>
    public interface IPluginConfig
    {

        /// <summary>
        /// 依赖插件数组,返回null时则自动生成依赖
        /// </summary>
        string[] DependOn { get; }

        /// <summary>
        /// 插件加载完成后调用
        /// </summary>
        /// <param name="app"></param>
        /// <param name="services"></param>
        /// <param name="plg"></param>
        void Loaded(ITAApplication app, IServiceCollection services, PluginObject plg);


        /// <summary>
        /// 插件御载处理
        /// </summary>
        /// <param name="app"></param>
        /// <param name="plg"></param>
        void Unload(ITAApplication app, PluginObject plg);
    }
}
