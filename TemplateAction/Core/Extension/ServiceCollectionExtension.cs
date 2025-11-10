using System;
using TemplateAction.Common;
namespace TemplateAction.Core
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection Clear<T1>(this IServiceCollection collection)
        {
            collection.Remove(typeof(T1).FullName);
            return collection;
        }
        #region 添加Scope
        public static IServiceCollection AddScope(this IServiceCollection collection, Type impType, Type serviceType)
        {
            collection.Add(impType.FullName, new ServiceDescriptor(serviceType, ServiceLifetime.Scope, null, null));
            return collection;
        }
        public static IServiceCollection TryAddScope(this IServiceCollection collection, Type impType, Type serviceType)
        {
            collection.TryAdd(impType.FullName, new ServiceDescriptor(serviceType, ServiceLifetime.Scope, null, null));
            return collection;
        }

        public static IServiceCollection AddScope<T1>(this IServiceCollection collection, ProxyFactory factory = null)
        {
            collection.Add(typeof(T1).FullName, new ServiceDescriptor(typeof(T1), ServiceLifetime.Scope, factory, null));
            return collection;
        }
        public static IServiceCollection TryAddScope<T1>(this IServiceCollection collection, ProxyFactory factory = null)
        {
            collection.TryAdd(typeof(T1).FullName, new ServiceDescriptor(typeof(T1), ServiceLifetime.Scope, factory, null));
            return collection;
        }
        public static IServiceCollection AddScope<T1, T2>(this IServiceCollection collection, ProxyFactory factory = null) where T2 : class
        {
            collection.Add(typeof(T1).FullName, new ServiceDescriptor(typeof(T2), ServiceLifetime.Scope, factory, null));
            return collection;
        }
        public static IServiceCollection TryAddScope<T1, T2>(this IServiceCollection collection, ProxyFactory factory = null) where T2 : class
        {
            collection.TryAdd(typeof(T1).FullName, new ServiceDescriptor(typeof(T2), ServiceLifetime.Scope, factory, null));
            return collection;
        }
        public static IServiceCollection AddScope(this IServiceCollection collection, string key, Type serviceType)
        {
            collection.Add(key, new ServiceDescriptor(serviceType, ServiceLifetime.Scope, null, null));
            return collection;
        }
        public static IServiceCollection TryAddScope(this IServiceCollection collection, string key, Type serviceType)
        {
            collection.TryAdd(key, new ServiceDescriptor(serviceType, ServiceLifetime.Scope, null, null));
            return collection;
        }
        #endregion



        #region 添加Transient
        public static IServiceCollection AddTransient<T1>(this IServiceCollection collection, ProxyFactory factory = null)
        {
            collection.Add(typeof(T1).FullName, new ServiceDescriptor(typeof(T1), ServiceLifetime.Transient, factory, null));
            return collection;
        }
        public static IServiceCollection TryAddTransient<T1>(this IServiceCollection collection, ProxyFactory factory = null)
        {
            collection.TryAdd(typeof(T1).FullName, new ServiceDescriptor(typeof(T1), ServiceLifetime.Transient, factory, null));
            return collection;
        }
        public static IServiceCollection AddTransient<T1, T2>(this IServiceCollection collection, ProxyFactory factory = null) where T2 : class
        {
            collection.Add(typeof(T1).FullName, new ServiceDescriptor(typeof(T2), ServiceLifetime.Transient, factory, null));
            return collection;
        }
        public static IServiceCollection TryAddTransient<T1, T2>(this IServiceCollection collection, ProxyFactory factory = null) where T2 : class
        {
            collection.TryAdd(typeof(T1).FullName, new ServiceDescriptor(typeof(T2), ServiceLifetime.Transient, factory, null));
            return collection;
        }
        public static IServiceCollection AddTransient(this IServiceCollection collection, Type impType, Type serviceType)
        {
            collection.Add(impType.FullName, new ServiceDescriptor(serviceType, ServiceLifetime.Transient, null, null));
            return collection;
        }
        public static IServiceCollection TryAddTransient(this IServiceCollection collection, Type impType, Type serviceType)
        {
            collection.TryAdd(impType.FullName, new ServiceDescriptor(serviceType, ServiceLifetime.Transient, null, null));
            return collection;
        }
        #endregion




        #region 添加Singleton

        public static IServiceCollection AddSingleton<T1, T2>(this IServiceCollection collection, ProxyFactory factory = null) where T2 : class
        {
            collection.Add(typeof(T1).FullName, new ServiceDescriptor(typeof(T2), ServiceLifetime.Singleton, factory, null));
            return collection;
        }
        public static IServiceCollection TryAddSingleton<T1, T2>(this IServiceCollection collection, ProxyFactory factory = null) where T2 : class
        {
            collection.TryAdd(typeof(T1).FullName, new ServiceDescriptor(typeof(T2), ServiceLifetime.Singleton, factory, null));
            return collection;
        }
        public static IServiceCollection AddSingleton<T1>(this IServiceCollection collection, ProxyFactory factory = null)
        {
            collection.Add(typeof(T1).FullName, new ServiceDescriptor(typeof(T1), ServiceLifetime.Singleton, factory, null));
            return collection;
        }
        public static IServiceCollection TryAddSingleton<T1>(this IServiceCollection collection, ProxyFactory factory = null)
        {
            collection.TryAdd(typeof(T1).FullName, new ServiceDescriptor(typeof(T1), ServiceLifetime.Singleton, factory, null));
            return collection;
        }
        public static IServiceCollection AddSingleton(this IServiceCollection collection, Type impType, Type serviceType)
        {
            collection.Add(impType.FullName, new ServiceDescriptor(serviceType, ServiceLifetime.Singleton, null, null));
            return collection;
        }

        public static IServiceCollection TryAddSingleton(this IServiceCollection collection, Type impType, Type serviceType, ProxyFactory factory = null)
        {
            collection.TryAdd(impType.FullName, new ServiceDescriptor(serviceType, ServiceLifetime.Singleton, null, null));
            return collection;
        }

        public static IServiceCollection AddSingleton<T1>(this IServiceCollection collection, object impInstance)
        {
            collection.Add(typeof(T1).FullName, new ServiceDescriptor(typeof(T1), ServiceLifetime.Singleton, null, impInstance));
            return collection;
        }
        public static IServiceCollection TryAddSingleton<T1>(this IServiceCollection collection, object impInstance)
        {
            collection.TryAdd(typeof(T1).FullName, new ServiceDescriptor(typeof(T1), ServiceLifetime.Singleton, null, impInstance));
            return collection;
        }
        public static IServiceCollection AddSingleton(this IServiceCollection collection, Type impType, object impInstance)
        {
            collection.Add(impType.FullName, new ServiceDescriptor(impType, ServiceLifetime.Singleton, null, impInstance));
            return collection;
        }
        public static IServiceCollection TryAddSingleton(this IServiceCollection collection, Type impType, object impInstance)
        {
            collection.TryAdd(impType.FullName, new ServiceDescriptor(impType, ServiceLifetime.Singleton, null, impInstance));
            return collection;
        }
        #endregion


        /// <summary>
        /// 注册字符串依赖
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="name"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static IServiceCollection AddString(this IServiceCollection collection, string name, string val)
        {
            Type strType = typeof(string);
            collection.Add(TAUtility.TypeName2ServiceKey(strType, name), new ServiceDescriptor(strType, ServiceLifetime.Singleton, null, val));
            return collection;
        }
    }
}
