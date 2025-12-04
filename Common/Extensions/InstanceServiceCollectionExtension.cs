using System;
using System.Linq;
using TemplateAction.Core;
using TemplateAction.NetCore;

namespace Common
{
    /// <summary>
    /// 拦截并注入DAL或BLL
    /// </summary>
    public static class InstanceServiceCollectionExtension
    {

        public static IServiceCollection AddBLL<T>(this IServiceCollection collection) where T : class
        {
            collection.AddTransient<T>((object[] arguments, ITAServiceProvider provider) =>
            {
                return MyAccess.Aop.InterceptFactory.CreateBLL<T>(arguments);
            });
            return collection;
        }
        public static IServiceCollection AddDAL<T>(this IServiceCollection collection) where T : class
        {
            collection.AddSingleton<T>((object[] arguments, ITAServiceProvider provider) =>
            {
                var dalObj = MyAccess.Aop.InterceptFactory.CreateDAL<T>(arguments);
                if (dalObj is IRepository repository)
                {
                    repository.Provider = provider;
                }
                return dalObj;
            });
            return collection;
        }
        public static IServiceCollection AddBLL<T1, T2>(this IServiceCollection collection) where T2 : class
        {
            collection.AddTransient<T1, T2>((object[] arguments, ITAServiceProvider provider) =>
             {
                 return MyAccess.Aop.InterceptFactory.CreateBLL<T2>(arguments);
             });
            return collection;
        }
        public static IServiceCollection AddDAL<T1, T2>(this IServiceCollection collection) where T2 : class
        {
            collection.AddSingleton<T1, T2>((object[] arguments, ITAServiceProvider provider) =>
            {
                var dalObj = MyAccess.Aop.InterceptFactory.CreateDAL<T2>(arguments);
                if (dalObj is IRepository repository)
                {
                    repository.Provider = provider;
                }
                return dalObj;
            });
            return collection;
        }
    }
}
