using Microsoft.Extensions.DependencyInjection;
using System;
using TemplateAction.Core;

namespace TemplateAction.NetCore
{
    /// <summary>
    /// 控制台程序专用的服务提供者创建工厂
    /// </summary>
    public class TANetConsoleServiceProviderFactory : IServiceProviderFactory<TAApplication>
    {
        private static volatile TAApplication _instance;
        private static object _lock = new object();
        private string _rootPath;
        private Action<TAApplication> _init;
        public TANetConsoleServiceProviderFactory(string rootPath, Action<TAApplication> init)
        {
            _rootPath = rootPath;
            _init = init;
        }
        public TAApplication CreateBuilder(Microsoft.Extensions.DependencyInjection.IServiceCollection services)
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        TAEventDispatcher.Instance.RegisterLoadBefore(_init);
                        _instance = new TANetConsoleApplication(services);
                        _instance.Init(_rootPath);
                    }
                }
            }

            return _instance;
        }

        public IServiceProvider CreateServiceProvider(TAApplication containerBuilder)
        {
            return new TANetServiceProvider(containerBuilder.ServiceProvider);
        }
    }
}
