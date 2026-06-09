using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;
using TemplateAction.Cache;

namespace TemplateAction.Core
{
    public class PluginObject
    {
        private IPluginConfig _config;
        public IPluginConfig Config { get { return _config; } }
        private string[] _dependOn;
        /// <summary>
        /// 生成依赖
        /// </summary>
        internal string[] GenerateDependOn()
        {
            if (_dependOn != null) return _dependOn;
            if (_config.DependOn == null)
            {
                //自动生成依赖
                List<string> tdeplist = new List<string>();
                AssemblyName[] names = _assembly.GetReferencedAssemblies();
                foreach (AssemblyName n in names)
                {
                    if (_collection.GetPlugin(n.Name) != null)
                    {
                        tdeplist.Add(n.Name);
                    }
                }
                _dependOn = tdeplist.ToArray();
            }
            else
            {
                _dependOn = _config.DependOn;
            }
            return _dependOn;
        }
        private string mName;
        public string Name
        {
            get { return mName; }
        }

        /// <summary>
        /// 当前插件版本
        /// </summary>
        private Version mVersion;
        public Version Ver { get { return mVersion; } }

        private IServiceCollection _services;
        public IServiceCollection Services
        {
            get { return _services; }
        }
        /// <summary>
        /// 插件的局部事件分发器
        /// </summary>
        private PluginEventDispatcher _dispatcher;
        public PluginEventDispatcher Dispatcher
        {
            get { return _dispatcher; }
        }

        private ConcurrentStorer _storer;
        /// <summary>
        /// 插件单例存储
        /// </summary>
        internal ConcurrentStorer Storer
        {
            get { return _storer; }
        }

        private Assembly _assembly;
        public Assembly TargetAssembly
        {
            get { return _assembly; }
        }

        private FileDependency _cacheDependency;
        public FileDependency CacheDependency
        {
            get { return _cacheDependency; }
        }
        private string _plgPath;
        public string PluginPath
        {
            get { return _plgPath; }
        }


        private ExtentionDataCollection _data;
        /// <summary>
        /// 插件扩展数据
        /// </summary>
        public ExtentionDataCollection Data
        {
            get { return _data; }
        }
        private PluginCollection _collection;
        public PluginCollection Collection
        {
            get { return _collection; }
        }
        private bool _isService;
        public bool IsService
        {
            get { return _isService; }
        }
        public PluginObject(PluginCollection collection, IPluginExtData pcdata, Assembly assembly, string pluginpath, bool isService)
        {
            this._isService = isService;
            this._collection = collection;
            this._assembly = assembly;
            this._plgPath = pluginpath;
            this._cacheDependency = new FileDependency();
            this._storer = new ConcurrentStorer();
            this._dispatcher = new PluginEventDispatcher();
            this.mName = assembly.GetName().Name;
            this._services = new ServiceCollection(collection, this.mName);
            this.mVersion = assembly.GetName().Version;
            this._data = new ExtentionDataCollection();
            if (pcdata != null && this.IsService)
            {
                pcdata.PluginLoadBefore(this);

                Type[] exports = this._assembly.GetExportedTypes();
                foreach (Type t in exports)
                {
                    //判断非抽像
                    if (!t.IsAbstract)
                    {
                        if (!pcdata.PluginLoadType(this, t))
                        {
                            if (this._config == null && typeof(IPluginConfig).IsAssignableFrom(t))
                            {
                                //执行插件配置文件
                                this._config = Activator.CreateInstance(t) as IPluginConfig;
                            }
                        }

                    }

                }
                pcdata.PluginLoadAfter(this);
            }
            else
            {
                Type[] exports = this._assembly.GetExportedTypes();
                foreach (Type t in exports)
                {
                    //判断非抽像
                    if (!t.IsAbstract)
                    {
                        if (this._config == null && typeof(IPluginConfig).IsAssignableFrom(t))
                        {
                            //执行插件配置文件
                            this._config = Activator.CreateInstance(t) as IPluginConfig;
                        }
                    }

                }
            }


            //无Config，则创建一个默认的
            if (this._config == null && pcdata != null)
            {
                this._config = new EmptyConfig();
            }
        }

        /// <summary>
        /// 查找服务
        /// </summary>
        /// <param name="implementation"></param>
        /// <returns></returns>
        public IServiceDescriptorEnumerable FindService(string key)
        {
            return _services[key];
        }
        /// <summary>
        /// 删除指定服务
        /// </summary>
        /// <param name="key"></param>
        public void RemoveService(string key)
        {
            _services.Remove(key);
        }
        /// <summary>
        /// 插件加载
        /// </summary>
        /// <param name="app"></param>
        public void Loaded(ITAApplication app)
        {
            _config.Loaded(app, this.Services, this);
        }

        /// <summary>
        /// 插件御载
        /// </summary>
        public void Unload(ITAApplication app)
        {
            _cacheDependency.NoticeChange();
            _config.Unload(app, this);
        }
    }
}
