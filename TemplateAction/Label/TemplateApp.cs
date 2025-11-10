using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using TemplateAction.Cache;
using TemplateAction.Common;
namespace TemplateAction.Label
{
    /// <summary>
    /// 模板应用：使用模板功能必需初始化此类,TAApplication已默认初始化
    /// </summary>
    public class TemplateApp
    {
        internal static readonly TemplateApp _instance = new TemplateApp();
        static TemplateApp() { }
        /// <summary>
        /// 内置函数
        /// </summary>
        private Dictionary<string, MethodInfo> mSysMethods;
        /// <summary>
        /// 模板编译缓存
        /// </summary>
        private CachePool _pool;
        //模板监听器
        private FileDependencyWatcher _watcher;
        private string _rootpath;
        private int _inited = 0;
        private TemplateApp()
        {
            _pool = new CachePool();
            mSysMethods = new Dictionary<string, MethodInfo>();
            //加载模板系统函数
            Type funType = typeof(TempFuns);
            MethodInfo[] methodArray = funType.GetMethods(BindingFlags.Public | BindingFlags.Static);
            foreach (MethodInfo method in methodArray)
            {
                mSysMethods.Add(method.Name, method);
            }
        }
        public static TemplateApp Instance
        {
            get
            {
                return _instance;
            }
        }
        /// <summary>
        /// 初始化模板目录
        /// </summary>
        /// <param name="path"></param>
        public void Init(string path)
        {
            if (_inited == 1)
            {
                return;
            }
            int orists = Interlocked.CompareExchange(ref _inited, 1, 0);
            if (orists != 0)
            {
                return;
            }
            _rootpath = path;
            _watcher = new FileDependencyWatcher(path, "*" + TAUtility.FILE_EXT);
        }
        /// <summary>
        /// 清除缓存
        /// </summary>
        public void CacheEmpty()
        {
            _pool.Empty();
        }
        /// <summary>
        /// 添加插件模板
        /// </summary>
        /// <param name="path"></param>
        /// <param name="input"></param>
        /// <param name="dep"></param>
        public void AddViewPage(string path, string input, FileDependency dep)
        {
            string tmppath = Path.Combine(_rootpath, path).ToLower();
            TemplateDocument filedoc = new TemplateDocument(input);
            _pool.Remove(tmppath);
            _pool.Insert(tmppath, filedoc, new UnionOrDependency(dep, _watcher.CreateFileDependency(tmppath)));
        }
        /// <summary>
        /// 加载模板原文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public TemplateDocument LoadViewPage(string relativePath)
        {
            string path = TAUtility.RelativeToAbsolutePath(_rootpath, relativePath);
            string lowpath = path.ToLower();
            TemplateDocument filedoc = _pool.Get(lowpath) as TemplateDocument;
            if (Equals(filedoc, null))
            {
                string filecont;
                int err = TAUtility.ReadFile(out filecont, path);
                if (err == 0)
                {
                    filedoc = new TemplateDocument(filecont);
                    FileDependency filedep = _watcher.CreateFileDependency(lowpath);
                    if (filedep != null)
                    {
                        _pool.Insert(lowpath, filedoc, filedep);
                    }
                }
                else
                {
                    return null;
                }
            }

            return filedoc;
        }

        /// <summary>
        /// 获取系统函数
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public MethodInfo GetSystemMethod(string name)
        {
            MethodInfo method;
            if (mSysMethods.TryGetValue(name, out method))
            {
                return method;
            }
            return null;
        }

 
    }
}
