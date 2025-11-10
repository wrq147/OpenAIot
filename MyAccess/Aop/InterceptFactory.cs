using Castle.DynamicProxy;
using System;
using System.Collections.Concurrent;
namespace MyAccess.Aop
{
    /// <summary>
    /// Aop代理工厂
    /// </summary>
    public class InterceptFactory
    {
        private static ConcurrentDictionary<string, ProxyGenerator> _cacheProxyGenerator = new ConcurrentDictionary<string, ProxyGenerator>();
        public static ProxyGenerator GetProxyGenerator(Type t)
        {
            string guid = t.Assembly.ManifestModule.ModuleVersionId.ToString();
            string tfullname = t.Assembly.GetHashCode() + guid;
            return _cacheProxyGenerator.GetOrAdd(tfullname, (string k) =>
            {
                return new ProxyGenerator();
            });
        }
        private static readonly IProxyGenerationHook _pubHook = new PublicGenerationHook();
        /// <summary>
        /// 创建DAL代理
        /// </summary>
        /// <param name="interfaceToProxy"></param>
        /// <param name="t"></param>
        /// <param name="constructorArguments"></param>
        /// <returns></returns>
        public static object CreateDAL(Type interfaceToProxy, Type t, object[] constructorArguments)
        {
            ProxyGenerationOptions option = new ProxyGenerationOptions(_pubHook);
            ProxyGenerator tpg = GetProxyGenerator(t);
            object target = Activator.CreateInstance(t, constructorArguments);
            return tpg.CreateInterfaceProxyWithTarget(interfaceToProxy, target, option, new DBIntercept());
        }

        /// <summary>
        /// 无接口创建DAL代理
        /// </summary>
        /// <param name="t"></param>
        /// <param name="constructorArguments"></param>
        /// <returns></returns>
        public static object CreateDAL(Type t, object[] constructorArguments)
        {
            ProxyGenerationOptions option = new ProxyGenerationOptions(_pubHook);
            ProxyGenerator tpg = GetProxyGenerator(t);
            return tpg.CreateClassProxy(t, option, constructorArguments, new DBIntercept());
        }

        /// <summary>
        /// 创建BLL代理
        /// </summary>
        /// <param name="interfaceToProxy"></param>
        /// <param name="t"></param>
        /// <param name="constructorArguments"></param>
        /// <returns></returns>
        public static object CreateBLL(Type interfaceToProxy, Type t, object[] constructorArguments)
        {
            ProxyGenerationOptions option = new ProxyGenerationOptions(_pubHook);
            ProxyGenerator tpg = GetProxyGenerator(t);
            object target = Activator.CreateInstance(t, constructorArguments);
            return tpg.CreateInterfaceProxyWithTarget(interfaceToProxy, target, option, new BLLIntercept());
        }
        /// <summary>
        /// 无接口创建BLL代理
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static object CreateBLL(Type t, object[] constructorArguments)
        {
            ProxyGenerationOptions option = new ProxyGenerationOptions(_pubHook);
            ProxyGenerator tpg = GetProxyGenerator(t);
            return tpg.CreateClassProxy(t, option, constructorArguments, new BLLIntercept());
        }


        private static readonly IProxyGenerationHook _entityHook = new EntityGenerationHook();

        /// <summary>
        /// 创建实体
        /// </summary>
        /// <typeparam name="T">被拦截实体的属性必需为virtual</typeparam>
        /// <returns></returns>
        public static T CreateEntityOp<T>() where T : class
        {
            ProxyGenerationOptions option = new ProxyGenerationOptions(_entityHook);
            BaseEntity be = new BaseEntity();
            option.AddMixinInstance(be);
            ProxyGenerator tpg = GetProxyGenerator(typeof(T));
            T rt = tpg.CreateClassProxy<T>(option, new EntitylIntercept());
            be.StartRecord();
            return rt;
        }

        public static object CreateEntityOp(Type t)
        {
            ProxyGenerationOptions option = new ProxyGenerationOptions(_entityHook);
            BaseEntity be = new BaseEntity();
            option.AddMixinInstance(be);
            ProxyGenerator tpg = GetProxyGenerator(t);
            object rt = tpg.CreateClassProxy(t, option, new EntitylIntercept());
            be.StartRecord();
            return rt;
        }

        /// <summary>
        /// 创建事件监听器的触发实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="callFun">事件触发函数(DispatchToListener方法)</param>
        /// <returns></returns>
        public static T CreateDispatcher<T>(Action<string, string, object[]> callFun) where T : class
        {
            Type interfaceT = typeof(T);
            ProxyGenerator tpg = GetProxyGenerator(interfaceT);
            return tpg.CreateInterfaceProxyWithoutTarget<T>(new ListenerIntercept(callFun));
        }
        /// <summary>
        /// 获取代理的实际类名
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetProxyTypeName(object obj)
        {
            if (ProxyUtil.IsProxy(obj))
            {
                return ProxyUtil.GetUnproxiedType(obj).Name;
            }
            else
            {
                return obj.GetType().Name;
            }
        }
        /// <summary>
        /// 获取代理的实际类型
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Type GetProxyType(object obj)
        {
            if (ProxyUtil.IsProxy(obj))
            {
                return ProxyUtil.GetUnproxiedType(obj);
            }
            else
            {
                return obj.GetType();
            }
        }
    }
}
