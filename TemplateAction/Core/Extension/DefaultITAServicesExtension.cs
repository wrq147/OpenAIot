using System;
using System.Collections.Generic;

namespace TemplateAction.Core
{
    public static class DefaultITAServicesExtension
    {
        public static object GetService(this ITAServiceProvider services, string key)
        {
            if (TAAction.Current != null)
            {
                return services.GetService(key, TAAction.Current);
            }
            else
            {
                return services.GetService(key);
            }
        }
        public static T GetService<T>(this ITAServiceProvider collection, ILifetimeFactory scopeFactory = null) where T : class
        {
            if (scopeFactory == null && TAAction.Current != null)
            {
                return collection.GetService(typeof(T), TAAction.Current) as T;
            }
            else
            {
                return collection.GetService(typeof(T), scopeFactory) as T;
            }
        }
        public static IEnumerable<T> GetServices<T>(this ITAServiceProvider collection, ILifetimeFactory scopeFactory = null) where T : class
        {
            if (scopeFactory == null && TAAction.Current != null)
            {
                return collection.GetService(typeof(IEnumerable<T>), TAAction.Current) as IEnumerable<T>;
            }
            else
            {
                return collection.GetService(typeof(IEnumerable<T>), scopeFactory) as IEnumerable<T>;
            }
        }

    }
}
