using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using TemplateAction.Core;
namespace TemplateAction.NetCore
{
    public static class TANetCoreServiceCollectionExtension
    {
    
        /// <summary>
        /// 微软内置服务转移到TA的服务中
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="tservices"></param>
        /// <returns></returns>
        public static IServiceCollection CopyServicesFrom(this IServiceCollection collection, Microsoft.Extensions.DependencyInjection.IServiceCollection tservices)
        {
            //复制服务
            foreach (Microsoft.Extensions.DependencyInjection.ServiceDescriptor micsd in tservices)
            {
                ServiceLifetime lifetime = ServiceLifetime.Singleton;
                switch (micsd.Lifetime)
                {
                    case Microsoft.Extensions.DependencyInjection.ServiceLifetime.Scoped:
                        lifetime = ServiceLifetime.Scope;
                        break;
                    case Microsoft.Extensions.DependencyInjection.ServiceLifetime.Transient:
                        lifetime = ServiceLifetime.Transient;
                        break;
                    case Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton:
                        lifetime = ServiceLifetime.Singleton;
                        break;
                }
                ProxyFactory pfactory = null;
                Func<IServiceProvider, object> func = micsd.ImplementationFactory;
                if (func != null)
                {
                    pfactory = (object[] constructorArguments, ITAServiceProvider provider) =>
                    {
                        return func.Invoke(provider.GetService<IServiceProvider>());
                    };
                    collection.Add(micsd.ServiceType.FullName, new ServiceDescriptor(micsd.ImplementationType ?? micsd.ServiceType, lifetime, pfactory, micsd.ImplementationInstance));
                }
                else
                {
                    collection.Add(micsd.ServiceType.FullName, new ServiceDescriptor(micsd.ImplementationType, lifetime, pfactory, micsd.ImplementationInstance));
                }

            }
            return collection;
        }
        /// <summary>
        /// 添加Microsoft.Extensions.DependencyInjection服务    
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="ac"></param>
        /// <returns></returns>
        public static IServiceCollection AddServices(this IServiceCollection collection, Action<Microsoft.Extensions.DependencyInjection.IServiceCollection> ac)
        {
            Microsoft.Extensions.DependencyInjection.IServiceCollection services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            ac.Invoke(services);
            collection.CopyServicesFrom(services);
            return collection;
        }
        public static IServiceCollection AddOptions(this IServiceCollection collection)
        {
            collection.TryAddSingleton(typeof(IOptions<>), typeof(OptionsManager<>));
            collection.TryAddScope(typeof(IOptionsSnapshot<>), typeof(OptionsManager<>));
            collection.TryAddSingleton(typeof(IOptionsMonitor<>), typeof(OptionsMonitor<>));
            collection.TryAddTransient(typeof(IOptionsFactory<>), typeof(OptionsFactory<>));
            collection.TryAddSingleton(typeof(IOptionsMonitorCache<>), typeof(OptionsCache<>));
            return collection;
        }
        public static IServiceCollection Configure<TOptions>(this IServiceCollection services, string name, Action<TOptions> configureOptions) where TOptions : class
        {
            services.AddOptions();
            services.AddSingleton<IConfigureOptions<TOptions>>(new ConfigureNamedOptions<TOptions>(name, configureOptions));
            return services;
        }
        public static IServiceCollection Configure<TOptions>(this IServiceCollection services, Action<TOptions> configureOptions) where TOptions : class
        {
            return services.Configure(string.Empty, configureOptions);
        }
        public static IServiceCollection Configure<TOptions>(this IServiceCollection services, string name, IConfiguration config) where TOptions : class
        {
            return services.Configure<TOptions>(name, config, _ => { });
        }
        public static IServiceCollection Configure<TOptions>(this IServiceCollection services, string name, IConfiguration config, Action<BinderOptions> configureBinder) where TOptions : class
        {
            services.AddOptions();
            services.AddSingleton<IOptionsChangeTokenSource<TOptions>>(new ConfigurationChangeTokenSource<TOptions>(name, config));
            services.AddSingleton<IConfigureOptions<TOptions>>(new NamedConfigureFromConfigurationOptions<TOptions>(name, config, configureBinder));
            return services;
        }
        public static IServiceCollection Configure<TOptions>(this IServiceCollection services, IConfiguration config) where TOptions : class
        {
            return services.Configure<TOptions>(string.Empty, config);
        }
        /// <summary>
        /// 添加TA的WebApi探测
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddTAWebApiExplorer(this IServiceCollection services, Action<ApiExplorerOptions> ac = null)
        {
            ApiExplorerOptions apiExplorerOpt = new ApiExplorerOptions();
            ac?.Invoke(new ApiExplorerOptions());
            services.TryAddSingleton<ApiExplorerOptions>(apiExplorerOpt);
            services.TryAddSingleton<IApiDescriptionGroupCollectionProvider, TANetWebApiDescriptionGroupCollectionProvider>();
            return services;
        }
    }
}
