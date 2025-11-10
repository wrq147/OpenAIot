using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using TemplateAction.Common;
using TemplateAction.Core;

namespace TemplateAction.NetCore
{
    public class TANetCoreHttpApplication : TASiteApplication, IHttpApplication<HttpContext>
    {
        private RequestDelegate _requestDelegate;
        private Microsoft.Extensions.DependencyInjection.ServiceCollection _service;
        private IApplicationBuilder _appBuilder;
        public IApplicationBuilder AppBuilder
        {
            get { return _appBuilder; }
        }
        private KestrelServerOptions _kestrelServerOptions;
        public KestrelServerOptions KestrelOptions
        {
            get { return _kestrelServerOptions; }
        }
        public TANetCoreHttpApplication(IApplicationBuilder appbuilder, KestrelServerOptions kestrelServerOptions, Microsoft.Extensions.DependencyInjection.ServiceCollection services)
        {
            _service = services;
            _appBuilder = appbuilder;
            _kestrelServerOptions = kestrelServerOptions;
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

        public HttpContext CreateContext(IFeatureCollection contextFeatures)
        {
            contextFeatures.Set<TANetCoreHttpApplication>(this);
            DefaultHttpContext df = new DefaultHttpContext(contextFeatures);
            df.RequestServices = this.ServiceProvider.GetService<IServiceProvider>();
            return df;
        }

        public void DisposeContext(HttpContext context, Exception exception)
        {
        }

        public async Task ProcessRequestAsync(HttpContext context)
        {
            await _requestDelegate(context);
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
            Services.CopyServicesFrom(_service);
            Services.AddScope<ITAContext>();
            Services.AddSingleton<IServiceScopeFactory, TANetCoreScopeFactory>();
            base.BeforeInit();
        }

        protected override void AfterInit()
        {
            TAAsyncHelper.RunSync(async () => {
                await TAEventDispatcher.Instance.Dispatch(_kestrelServerOptions).ConfigureAwait(false);
            });
            _requestDelegate = _appBuilder.Build();
            base.AfterInit();
        }
    }
}
