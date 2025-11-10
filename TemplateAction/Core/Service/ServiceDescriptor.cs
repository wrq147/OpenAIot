using System;

namespace TemplateAction.Core
{

    /// <summary>
    /// 代理服务工厂
    /// </summary>
    /// <param name="constructorArguments"></param>
    /// <returns></returns>
    public delegate object ProxyFactory(object[] constructorArguments, ITAServiceProvider provider);
    /// <summary>
    /// 服务描述信息
    /// </summary>
    public class ServiceDescriptor
    {
        private static int IncreaseId = 0;
        private int _id;
        public override int GetHashCode()
        {
            return _id;
        }
        public ServiceLifetime Lifetime { get; }
        public Type ServiceType { get; }
        public ProxyFactory Factory { get; }
        /// <summary>
        /// 直接指定实例
        /// </summary>
        public object Instance { get; }
        /// <summary>
        /// 所属插件
        /// </summary>
        public string PluginName { get; set; }

        public ServiceDescriptor(Type serviceType, ServiceLifetime lifetime, ProxyFactory factory, object instance)
        {
            _id = ++IncreaseId;
            ServiceType = serviceType;
            Lifetime = lifetime;
            Factory = factory;
            Instance = instance;
        }
    }
    public enum ServiceLifetime
    {
        /// <summary>
        /// 每次都获取相同一个实例,如果注册在插件上，则实例保存在插件上，插件更新时会跟着更新
        /// </summary>
        Singleton,
        /// <summary>
        /// 每次都获取一个新的实例
        /// </summary>
        Transient,
        /// <summary>
        /// 指定的域生命周期
        /// </summary>
        Scope
    }
}
