using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Reflection;
using TemplateAction.Label;
using TemplateAction.Cache;
using TemplateAction.Common;

namespace TemplateAction.Core
{
    /// <summary>
    /// 加载所有的插件
    /// </summary>
    public abstract class TAAbstractApplication : IDisposable, ITAApplication
    {
        private bool _disposed = false;
        private int _loaded = 0;
        /// <summary>
        /// 插件集
        /// </summary>
        private PluginCollection _plugins;
        protected PluginCollection Plugins
        {
            get { return _plugins; }
        }
        private PluginObject _entryPlugin;
        //监控插件更改
        private FileSystemWatcher _watcher;
        private HashedWheelTimer _timer = null;
        private string _rootPath;
        /// <summary>
        /// 根目录
        /// </summary>
        public string RootPath
        {
            get { return _rootPath; }
        }
        /// <summary>
        /// 插件目录
        /// </summary>
        private string _pluginPath;
        public string PluginPath
        {
            get { return _pluginPath; }
        }
        protected void SetPluginPath(string path)
        {
            _pluginPath = path;
        }
        /// <summary>
        /// 获取入口插件服务
        /// </summary>
        protected IServiceCollection Services
        {
            get { return _entryPlugin.Services; }
        }
        public ITAServiceProvider ServiceProvider
        {
            get { return _plugins; }
        }


        ~TAAbstractApplication()
        {
            Dispose(false);
        }

        private IPluginExtDataFactory _extDataFactory;
        protected void SetPluginExtFactory(IPluginExtDataFactory factory)
        {
            _extDataFactory = factory;
        }

        private ILoaderFactory _loaderFactory;
        protected void SetLoaderFactory(ILoaderFactory factory)
        {
            _loaderFactory = factory;
        }
        /// <summary>
        /// 映射根目录
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string MapRootPath(string path)
        {
            return TAUtility.RelativeToAbsolutePath(_rootPath, path);
        }

        protected virtual void PluginLoad(PluginObject plg)
        {
            plg.Loaded(this);
        }
        protected virtual void PluginUnload(PluginObject plg)
        {
            plg.Unload(this);
            PushConcurrentTask(() =>
            {
                _loader.UnloadAssembly(plg.TargetAssembly);
            }, TimeSpan.FromMinutes(1));
        }
        public void UsePlugin(PluginObject newObj)
        {
            _plugins.UsePlugin(newObj);
        }
        /// <summary>
        /// 显示释放对象资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed) return;
            _disposed = true;
            TAEventDispatcher.Instance.RemoveScope(_plugins);
            _watcher.Dispose();
            _timer.Stop();
            if (disposing)
            {
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// 卸载指定插件
        /// </summary>
        /// <param name="ns"></param>
        public void UnloadPlugin(string ns)
        {
            PushConcurrentTask(() =>
            {
                _plugins.RemovePlugin(ns);
            });

        }

        /// <summary>
        /// 判断是否包含指定名称的插件
        /// </summary>
        /// <param name="ns"></param>
        /// <returns></returns>
        public bool ExistPlugin(string ns)
        {
            return _plugins.ExistPlugin(ns);
        }

        private IPluginLoader _loader;
        protected IPluginLoader Loader
        {
            get { return _loader; }
        }

        /// <summary>
        /// 初始化前
        /// </summary>
        protected virtual void BeforeInit() { }
        /// <summary>
        /// 初始化后
        /// </summary>
        protected virtual void AfterInit() { }
        /// <summary>
        /// 初始化并加载目录下的插件
        /// </summary>
        /// <param name="rootpath"></param>
        /// <param name="entry">入口程序集</param>
        protected void InitApplication(string rootpath, Assembly entry)
        {
            if (rootpath.Length > 0 && rootpath[rootpath.Length - 1] == Path.DirectorySeparatorChar)
            {
                rootpath = rootpath.Substring(0, rootpath.Length - 1);
            }
            if (_loaded == 1)
            {
                return;
            }

            if (Interlocked.CompareExchange(ref _loaded, 1, 0) != 0)
            {
                return;
            }

            //初始化插件容器
            _plugins = new PluginCollection(_extDataFactory);
            TAEventDispatcher.Instance.AddScope(_plugins);

            //初始化根目录
            _rootPath = rootpath;
            //默认插件路径
            _pluginPath = Path.Combine(_rootPath, "Plugin");
            //初始化定时器
            _timer = new HashedWheelTimer(TimeSpan.FromMilliseconds(400), 1024, 0);

            //从应用程序域的程序集中初始化插件集
            if (entry == null)
            {
                throw new ArgumentNullException("入口程序集不能为null");
            }

            _entryPlugin = _plugins.NewPlugin(entry, null, true);
            BeforeInit();
            //监听插件事件
            TAEventDispatcher.Instance.RegisterPluginLoad(PluginLoad);
            TAEventDispatcher.Instance.RegisterPluginUnload(PluginUnload);
            //先初始化入口插件
            _plugins.UsePlugin(_entryPlugin);
            TAEventDispatcher.Instance.DispathPluginLoad(_entryPlugin);
            //加载插件
            if (_loaderFactory == null)
            {
                _loaderFactory = new DefaultLoaderFactory();
            }
            _loader = _loaderFactory.CreateLoader(_plugins);
            _loader.LoadFrom(entry, _pluginPath);

            TAEventDispatcher.Instance.DispathPluginAllLoad();
            AfterInit();
            //开启监控插件更改
            _watcher = new FileSystemWatcher();
            _watcher.Filter = "*" + TAUtility.ModExt;
            _watcher.NotifyFilter = NotifyFilters.LastWrite;
            _watcher.Path = _pluginPath;
            _watcher.EnableRaisingEvents = true;
            _watcher.IncludeSubdirectories = true;
            _watcher.Changed += OnPluginListener;

            return;
        }

        #region 插件监听代码
        /// <summary>
        /// 卸载所有插件
        /// </summary>
        public void UnloadAllPlugin()
        {
            _plugins.RemoveAllPlugin();
        }
        /// <summary>
        /// 压入同步任务
        /// </summary>
        /// <param name="ac"></param>
        public void PushConcurrentTask(Action ac)
        {
            PushConcurrentTask(ac, TimeSpan.Zero);
        }
        /// <summary>
        /// 压入同步任务
        /// </summary>
        /// <param name="ac"></param>
        /// <param name="ts"></param>
        public void PushConcurrentTask(Action ac, TimeSpan ts)
        {
            _timer.NewTimeout(new ConcurrentTask(ac), ts);
        }
        private static HashSet<string> _changePaths = new HashSet<string>();
        private void OnPluginListener(object sender, FileSystemEventArgs e)
        {
            string tpath = e.FullPath;
            lock (_changePaths)
            {
                if (!_changePaths.Contains(tpath))
                {
                    _changePaths.Add(tpath);
                }
                _timer.NewTimeout(new ConcurrentTask(OnWatchedFileChange), TimeSpan.FromMilliseconds(500));
            }
        }
        private void OnWatchedFileChange()
        {
            List<string> backup = new List<string>();
            lock (_changePaths)
            {
                backup.AddRange(_changePaths);
                _changePaths.Clear();
            }

            if (backup.Count > 0)
            {
                _loader.UpdateAssembly(backup);
            }
        }


        #endregion
    }
}
