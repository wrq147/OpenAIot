using AspectCore.Configuration;
using AspectCore.DynamicProxy;
using MyAccess.Aop.Attribute;
using System;
using System.Collections.Concurrent;

namespace MyAccess.Aop
{
    public class InterceptFactory
    {
        private static ConcurrentDictionary<string, IProxyGenerator> _cacheProxyGenerator = new ConcurrentDictionary<string, IProxyGenerator>();
        public static IProxyGenerator GetProxyGenerator<T>(Action<IAspectConfiguration> options = null)
        {
            var t = typeof(T);
            string guid = t.Assembly.ManifestModule.ModuleVersionId.ToString();
            var typeName = t.FullName ?? t.Name;
            string tfullname = $"{t.Assembly.GetHashCode()}_{guid}_{typeName}";
            return _cacheProxyGenerator.GetOrAdd(tfullname, (string k) =>
            {
                ProxyGeneratorBuilder proxyGeneratorBuilder = new ProxyGeneratorBuilder();
                proxyGeneratorBuilder.Configure(options);
                return proxyGeneratorBuilder.Build();
            });
        }
        public static T CreateDAL<T>(object[] constructorArguments) where T : class
        {
            IProxyGenerator tpg = GetProxyGenerator<T>(options =>
            {
                options.Interceptors.AddTyped<DALAopAttr>(method =>
                {
                    return method.ReflectedType == typeof(T) && method.IsPublic && !method.IsStatic && !method.IsConstructor;
                });
            });
            return tpg.CreateClassProxy<T>(constructorArguments);
        }
        public static T CreateBLL<T>(object[] constructorArguments) where T : class
        {
            IProxyGenerator tpg = GetProxyGenerator<T>();
            return tpg.CreateClassProxy<T>(constructorArguments);
        }

    }
}
