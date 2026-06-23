using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using TemplateAction.Core;

namespace TemplateAction.NetCore
{
    public class TANetCoreHttpApplication : TASiteApplication
    {
        private IApplicationBuilder _appBuilder;
        public IApplicationBuilder AppBuilder
        {
            get { return _appBuilder; }
        }

        private Microsoft.Extensions.DependencyInjection.IServiceCollection _sc;
        public TANetCoreHttpApplication(IApplicationBuilder appBuilder, Microsoft.Extensions.DependencyInjection.IServiceCollection sc)
        {
            _appBuilder = appBuilder;
            _sc = sc;
            SetLoaderFactory(new TANetLoaderFactory());
        }

        /// <summary>
        /// 是否允许释放插件内存
        /// </summary>
        /// <returns></returns>
        public TANetCoreHttpApplication UseMemoryUnload()
        {
            TANetCorePluginLoader tmp = this.Loader as TANetCorePluginLoader;
            tmp?.UseMemoryUnload();
            return this;
        }

        public IServiceProvider GetServiceProvider()
        {
            return this.ServiceProvider.GetService<IServiceProvider>();
        }

        protected override void PluginLoad(PluginObject plg)
        {
            base.PluginLoad(plg);
            TANetWebApiDescriptionGroupCollectionProvider provider = ServiceProvider.GetService<IApiDescriptionGroupCollectionProvider>() as TANetWebApiDescriptionGroupCollectionProvider;
            if (provider != null)
            {
                provider.ClearCache();
            }
        }
        protected override void BeforeInit()
        {
            //映射默认服务
            Services.AddTransient<ServiceDescriptorList>((object[] constructorArguments, ITAServiceProvider provider) =>
            {
                //获取当前注入的所有服务
                var list = ServiceDescriptorList.Create();
                Plugins.CopyServiceTo(list);
                return list;
            });
            Services.TryAddSingleton<IServiceProvider, TANetServiceProvider>();
            Services.TryAddSingleton<IServiceProviderIsService, TANetServiceProvider>();
            Services.CopyServicesFrom(_sc);
            Services.AddScope<ITAContext>();
            Services.AddSingleton<IServiceScopeFactory, TANetCoreScopeFactory>();
            base.BeforeInit();
        }

        protected override void AfterInit()
        {
            base.AfterInit();
        }
    }
}
