using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Collections.Concurrent;
using System.IO;
using TemplateAction.Common;
using System.Threading.Tasks;

namespace TemplateAction.Core
{
    /// <summary>
    /// 插件集合
    /// </summary>
    public class PluginCollection : ITAServiceProvider, IDispatcher, IInstanceFactory
    {
        private Dictionary<string, PluginObject> mPluginList = new Dictionary<string, PluginObject>();
        private ReaderWriterLockSlim _lockslim = new ReaderWriterLockSlim();


        private IPluginExtData _extionData;
        public IPluginExtData ExtentionData
        {
            get { return _extionData; }
        }
        /// <summary>
        /// 单服务搜索缓存
        /// </summary>
        private ConcurrentDictionary<string, string> _keycache = new ConcurrentDictionary<string, string>();
        /// <summary>
        /// 多服务搜索缓存
        /// </summary>
        private ConcurrentDictionary<string, string[]> _mulkeycache = new ConcurrentDictionary<string, string[]>();
        public PluginCollection(IPluginExtDataFactory extionData)
        {
            _extionData = extionData?.CreateExtData();
        }
        public bool ExistService(string key)
        {
            PluginObject[] tarr = GetAllPlugin();
            foreach (PluginObject plg in tarr)
            {
                IServiceDescriptorEnumerable des = plg.FindService(key);
                if (des != null)
                {
                    return true;
                }
            }
            return false;
        }
        public void ClearService(string key)
        {
            PluginObject[] tarr = GetAllPlugin();
            foreach (PluginObject plg in tarr)
            {
                plg.RemoveService(key);
            }
        }
        public void CopyServiceTo(ServiceDescriptorList list)
        {
            PluginObject[] tarr = GetAllPlugin();
            foreach (PluginObject plg in tarr)
            {
                plg.Services.CopyTo(list);
            }
        }
        private IServiceDescriptorEnumerable FindServicesIn(string key)
        {
            List<string> tnslist = new List<string>();
            ServiceDescriptorUnion deslist = new ServiceDescriptorUnion();

            PluginObject[] tarr = GetAllPlugin();
            foreach (PluginObject plg in tarr)
            {
                IServiceDescriptorEnumerable des = plg.FindService(key);
                if (des != null)
                {
                    deslist.Union(des);
                    tnslist.Add(plg.Name);
                }
            }
            _mulkeycache.TryAdd(key, tnslist.ToArray());
            return deslist;
        }
        /// <summary>
        /// 通过接口查找多个服务
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private IServiceDescriptorEnumerable FindServices(string key)
        {
            ServiceDescriptorUnion deslist = new ServiceDescriptorUnion();
            string[] nsArr;
            if (_mulkeycache.TryGetValue(key, out nsArr))
            {
                foreach (string ns in nsArr)
                {
                    PluginObject pobj = GetPlugin(ns);
                    IServiceDescriptorEnumerable des = pobj.FindService(key);
                    if (des != null)
                    {
                        deslist.Union(des);
                    }
                    else
                    {
                        _mulkeycache.TryRemove(key, out nsArr);
                        return FindServicesIn(key);
                    }
                }
                return deslist;
            }
            return FindServicesIn(key);
        }
        private IServiceDescriptorEnumerable FindServiceIn(string key)
        {
            //在插件中查找
            IServiceDescriptorEnumerable returnDesc = null;
            PluginObject[] tarr = GetAllPlugin();
            foreach (PluginObject plg in tarr)
            {
                returnDesc = plg.FindService(key);
                if (returnDesc != null)
                {
                    _keycache.TryAdd(key, plg.Name);
                    return returnDesc;
                }
            }
            return returnDesc;
        }
        /// <summary>
        /// 通过接口查找单个服务
        /// </summary>
        /// <param name="key">接口类型</param>
        /// <returns>服务描述信息</returns>
        private IServiceDescriptorEnumerable FindService(string key)
        {
            IServiceDescriptorEnumerable returnDesc;
            string targetns;
            if (_keycache.TryGetValue(key, out targetns))
            {
                PluginObject pobj = GetPlugin(targetns);
                returnDesc = pobj.FindService(key);
                if (returnDesc != null)
                {
                    return returnDesc;
                }
                else
                {
                    _keycache.TryRemove(key, out targetns);
                    return FindServiceIn(key);
                }
            }
            else
            {
                return FindServiceIn(key);
            }

        }

        /// <summary>
        /// 获取服务实例
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object GetService(string key, ILifetimeFactory scopeFactory = null)
        {
            IServiceDescriptorEnumerable sd = FindService(key);
            if (sd == null)
            {
                return null;
            }
            return Des2Instance(sd.First, scopeFactory);
        }
        public object GetService(Type tp, ILifetimeFactory scopeFactory = null)
        {
            if (tp.IsGenericType && !tp.IsGenericTypeDefinition)
            {
                Type defType = tp.GetGenericTypeDefinition();
                if (typeof(IEnumerable<>) == defType)
                {
                    //获取服务集
                    Type destType = tp.GetGenericArguments()[0];
                    Type listType = typeof(List<>).MakeGenericType(destType);
                    object list = Activator.CreateInstance(listType);
                    IServiceDescriptorEnumerable sdenum = FindServices(destType.FullName);
                    MethodInfo addMethod = listType.GetMethod("Add");
                    foreach (ServiceDescriptor sd in sdenum)
                    {
                        if (sd != null)
                        {
                            addMethod.Invoke(list, new object[] { Des2Instance(sd, scopeFactory) });
                        }
                    }
                    return list;
                }
                else
                {
                    IServiceDescriptorEnumerable sdenum = FindService(tp.FullName);
                    if (sdenum == null)
                    {
                        //使用泛型定义搜索
                        sdenum = FindService(tp.GetGenericTypeDefinition().FullName);
                        if (sdenum == null) return null;
                        ServiceDescriptor sd = sdenum.First;
                        return Des2Instance(sd, sd.ServiceType.MakeGenericType(tp.GetGenericArguments()), scopeFactory);
                    }
                    return Des2Instance(sdenum.First, scopeFactory);
                }

            }
            else
            {
                IServiceDescriptorEnumerable sdenum = FindService(tp.FullName);
                if (sdenum == null) return null;
                return Des2Instance(sdenum.First, scopeFactory);
            }
        }

        /// <summary>
        /// 创建外部注入服务
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="scopeFactory"></param>
        /// <returns></returns>
        public object CreateScopeService(ILifetimeFactory scopeFactory, Type serviceType)
        {
            return scopeFactory.GetValue(this, serviceType, null);
        }
        private object Des2Instance(ServiceDescriptor sd, ILifetimeFactory scopeFactory)
        {
            return Des2Instance(sd, sd.ServiceType, scopeFactory);
        }

        /// <summary>
        /// ServiceDescriptor转实例
        /// </summary>
        /// <param name="plgname"></param>
        /// <param name="lifetime"></param>
        /// <param name="serviceType"></param>
        /// <param name="factory"></param>
        /// <param name="impInstance"></param>
        /// <param name="scopeFactory"></param>
        /// <returns></returns>
        private object Des2Instance(ServiceDescriptor sd, Type serviceType, ILifetimeFactory scopeFactory)
        {
            if (sd.Instance != null) return sd.Instance;
            object result = null;
            switch (sd.Lifetime)
            {
                case ServiceLifetime.Singleton:
                    {
                        PluginObject pobj = GetPlugin(sd.PluginName);
                        if (!Equals(pobj, null))
                        {
                            string tkey = serviceType.FullName;
                            if (sd.Factory != null)
                            {
                                tkey += sd.GetHashCode();
                            }
                            ConcurrentProxy proxy = pobj.Storer.GetOrAdd(tkey);
                            result = proxy.GetValue(this, serviceType, sd.Factory, scopeFactory);
                        }
                    }
                    break;
                case ServiceLifetime.Transient:
                    {
                        result = CreateServiceInstance(serviceType, sd.Factory, scopeFactory);
                    }
                    break;
                case ServiceLifetime.Scope:
                    {
                        if (scopeFactory != null)
                        {
                            result = scopeFactory.GetValue(this, serviceType, sd);
                        }
                        else
                        {
                            PluginObject pobj = GetPlugin(sd.PluginName);
                            if (!Equals(pobj, null))
                            {
                                string tkey = serviceType.FullName;
                                if (sd.Factory != null)
                                {
                                    tkey += sd.GetHashCode();
                                }
                                ConcurrentProxy proxy = pobj.Storer.GetOrAdd(tkey);
                                result = proxy.GetValue(this, serviceType, sd.Factory, scopeFactory);
                            }
                        }
                    }
                    break;
            }
            return result;
        }

        /// <summary>
        /// 创建服务实例
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public object CreateServiceInstance(Type serviceType, ProxyFactory factory, ILifetimeFactory scopeFactory)
        {
            if (serviceType == null) return factory(Array.Empty<object>(), this);
            //接口则直接调用factory无参构造
            if (serviceType.IsInterface && factory != null)
            {
                return factory(Array.Empty<object>(), this);
            }
            else if (serviceType.IsPrimitive || serviceType == typeof(string))
            {
                //string或基本类型返回
                if (factory != null)
                {
                    return factory(Array.Empty<object>(), this);
                }
                else
                {
                    return Activator.CreateInstance(serviceType);
                }
            }

            ConstructorInfo[] constructors = serviceType.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
            if (constructors.Length > 0)
            {
                //开始选择构造函数
                ConstructorInfo activationConstructor = null;
                object[] parameters = null;
                string err_param_name = null;
                foreach (ConstructorInfo tmpConstructor in constructors)
                {
                    err_param_name = null;
                    ParameterInfo[] tmpParameterInfos = tmpConstructor.GetParameters();
                    object[] tmpParameters = new object[tmpParameterInfos.Length];

                    for (int i = 0; i < tmpParameterInfos.Length; i++)
                    {
                        ParameterInfo parameter = tmpParameterInfos[i];
                        Type parameterType = parameter.ParameterType;

                        if (parameterType == typeof(ITAServiceProvider))
                        {
                            tmpParameters[i] = this;
                        }
                        else if (parameterType.IsPrimitive || parameterType == typeof(string))
                        {
                            tmpParameters[i] = GetService(TAUtility.TypeName2ServiceKey(parameterType, parameter.Name), scopeFactory);
                        }
                        else
                        {
                            tmpParameters[i] = GetService(parameterType, scopeFactory);
                            if (tmpParameters[i] == null)
                            {
                                if (parameter.HasDefaultValue)
                                {
                                    tmpParameters[i] = parameter.DefaultValue;
                                }
                                else
                                {
                                    err_param_name = parameter.Name;
                                    break;
                                }
                            }
                        }
                    }
                    if (err_param_name == null)
                    {
                        if (activationConstructor == null)
                        {
                            activationConstructor = tmpConstructor;
                            parameters = tmpParameters;
                        }
                        else if (parameters.Length < tmpParameters.Length)
                        {
                            activationConstructor = tmpConstructor;
                            parameters = tmpParameters;
                        }
                    }
                }

                if (activationConstructor == null)
                {
                    throw new ArgumentException(string.Format("{0} 构造函数的参数无法注入,请确保 {1} 已加入服务容器", serviceType.Name, err_param_name));
                }

                if (factory != null)
                {
                    return factory(parameters, this);
                }
                else
                {
                    return activationConstructor.Invoke(parameters);
                }
            }
            else
            {
                if (factory != null)
                {
                    return factory(Array.Empty<object>(), this);
                }
                else
                {
                    return Activator.CreateInstance(serviceType);
                }
            }
        }
        /// <summary>
        /// 判断是否存在指定插件
        /// </summary>
        /// <param name="ns"></param>
        /// <returns></returns>
        public bool ExistPlugin(string ns)
        {
            _lockslim.EnterReadLock();
            try
            {
                return mPluginList.ContainsKey(ns);
            }
            finally
            {
                _lockslim.ExitReadLock();
            }
        }
        /// <summary>
        /// 缓存插件集
        /// </summary>
        private PluginObject[] _cachePlugins;
        /// <summary>
        /// 刷新插件的缓存数据
        /// </summary>
        private void RefreshCachePlugins()
        {
            _mulkeycache.Clear();
            _keycache.Clear();
            PluginObject[] tarr = null;
            _lockslim.EnterWriteLock();
            try
            {
                tarr = new PluginObject[mPluginList.Count];
                mPluginList.Values.CopyTo(tarr, 0);
                _cachePlugins = tarr;
            }
            finally
            {
                _lockslim.ExitWriteLock();
            }
        }
        /// <summary>
        /// 获取所有插件
        /// </summary>
        /// <returns></returns>
        public PluginObject[] GetAllPlugin()
        {
            if (_cachePlugins == null)
            {
                RefreshCachePlugins();
            }
            return _cachePlugins;
        }
        /// <summary>
        /// 获取指定插件
        /// </summary>
        /// <param name="ns"></param>
        /// <returns></returns>
        public PluginObject GetPlugin(string ns)
        {
            _lockslim.EnterReadLock();
            try
            {
                PluginObject pobj = null;
                if (mPluginList.TryGetValue(ns.ToLower(), out pobj))
                {
                    return pobj;
                }
                return null;
            }
            finally
            {
                _lockslim.ExitReadLock();
            }
        }


        /// <summary>
        /// 激活插件
        /// </summary>
        /// <param name="newObj"></param>
        public void UsePlugin(PluginObject newObj)
        {
            PluginObject oldPlugin;
            _lockslim.EnterWriteLock();
            try
            {
                string keylow = newObj.Name.ToLower();
                mPluginList.TryGetValue(keylow, out oldPlugin);
                mPluginList[keylow] = newObj;
            }
            finally
            {
                _lockslim.ExitWriteLock();
            }
            RefreshCachePlugins();
            if (oldPlugin != null)
            {
                TAEventDispatcher.Instance.DispathPluginUnload(oldPlugin);
            }
        }




        /// <summary>
        /// 创建插件
        /// </summary>
        /// <param name="ass"></param>
        /// <param name="filepath"></param>
        /// <param name="isService"></param>
        /// <returns></returns>
        public PluginObject NewPlugin(Assembly ass, string filepath, bool isService)
        {
            if (ass == null) return null;
            PluginObject newObj = new PluginObject(this, _extionData, ass, filepath, isService);
            if (newObj.Config == null) return null;
            return newObj;
        }
        /// <summary>
        /// 移除所有插件
        /// </summary>
        public void RemoveAllPlugin()
        {
            foreach (var tmpkvp in mPluginList)
            {
                TAEventDispatcher.Instance.DispathPluginUnload(tmpkvp.Value);
            }
        }
        /// <summary>
        /// 移除指定插件
        /// </summary>
        /// <param name="ns"></param>
        public void RemovePlugin(string ns)
        {
            PluginObject oldPlugin;
            _lockslim.EnterWriteLock();
            try
            {
                mPluginList.TryGetValue(ns.ToLower(), out oldPlugin);
                mPluginList.Remove(oldPlugin.Name.ToLower());
            }
            finally
            {
                _lockslim.ExitWriteLock();
            }
            RefreshCachePlugins();
            if (oldPlugin != null && oldPlugin.PluginPath != null)
            {
                TAEventDispatcher.Instance.DispathPluginUnload(oldPlugin);
                try
                {
                    File.Delete(oldPlugin.PluginPath);
                }
                catch { }
            }
        }


        public async Task Dispatch<T>(string key, T evt) where T : class
        {
            PluginObject[] tarr = GetAllPlugin();
            foreach (PluginObject plg in tarr)
            {
                await plg.Dispatcher.Dispatch(key, evt);
            }
        }

        public async Task<Z> DispathWait<T, Z>(string key, T evt)
            where T : ResponseEvent
            where Z : EvtResponse, new()
        {
            PluginObject[] tarr = GetAllPlugin();
            foreach (PluginObject plg in tarr)
            {
                var rs = await plg.Dispatcher.DispathWait<T, Z>(key, evt);
                if (rs != null && rs.IsDone)
                {
                    return rs;
                }
            }
            return null;
        }
    }
}
