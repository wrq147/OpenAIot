using System;
using System.Collections.Generic;
using FluentMigrator.Infrastructure;
using FluentMigrator.Runner.Initialization;
using Microsoft.Extensions.DependencyInjection;
using TemplateAction.Core;
using System.Linq;

namespace Common.FluentMigrator
{
    public static class FluentMigratorExtensions
    {
        /// <summary>
        /// 配置FluentMigrator
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static Microsoft.Extensions.DependencyInjection.IServiceCollection AddTAFluent(this Microsoft.Extensions.DependencyInjection.IServiceCollection services)
        {
            //扫描TA模块的程序集
            services.AddSingleton<IAssemblySource, TAAssemblySource>();
            services.AddSingleton<IEmbeddedResourceProvider, TAEmbeddedResourceProvider>();
            services.AddFluentMigratorCore();

            services.AddSingleton<TAMigrateIntance>();
            return services;
        }

        /// <summary>
        /// 使用数据库版本控制
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static ITAApplication UseTAFluent(this ITAApplication app)
        {

            TAEventDispatcher.Instance.RegisterPluginLoad(plg =>
            {
                if (Constants.General != null && Constants.General.quick_init != true)
                {
                    TAAssemblySource taAss = app.ServiceProvider.GetService<IAssemblySource>() as TAAssemblySource;
                    if (taAss != null)
                    {
                        taAss.SetAssemblySource(plg.TargetAssembly);
                        var resProviders = app.ServiceProvider.GetService<IEnumerable<IEmbeddedResourceProvider>>();
                        var taResProvider = (TAEmbeddedResourceProvider)resProviders.Where(x => x is TAEmbeddedResourceProvider).FirstOrDefault();
                        if (taResProvider != null)
                        {
                            taResProvider.AddAssembly(plg.TargetAssembly);
                        }

                        TAMigrateIntance instance = app.ServiceProvider.GetService<TAMigrateIntance>();
                        if (instance != null)
                        {
                            instance.ToLast();
                        }
                    }
                }

            });
            return app;
        }
    }
}
