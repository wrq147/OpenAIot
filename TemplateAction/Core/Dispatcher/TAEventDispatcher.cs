using System;
using System.Reflection;
using System.Collections.Generic;
using System.Threading.Tasks;
using TemplateAction.Common;

namespace TemplateAction.Core
{
    /// <summary>
    /// 全局事件分发器
    /// </summary>
    public class TAEventDispatcher : AbstractEventDispatcher
    {
        private List<IDispatcher> _scopelist = new List<IDispatcher>();
        private Dictionary<string, ITAEventHandler> _handlers;
        public const string BEFORE_EVENT = "TA_BEFORE_LOAD";
        public const string PLUGIN_LOAD_EVENT = "TA_PLUGIN_LOAD_EVENT";
        public const string PLUGIN_UNLOAD_EVENT = "TA_PLUGIN_UNLOAD_EVENT";
        public const string PLUGIN_ALL_LOAD_EVENT = "TA_PLUGIN_ALL_LOAD_EVENT";
        private DefaultMultiHandler<PluginObject> _loadHandlers;
        private DefaultMultiHandler<string> _allLoadHandlers;
        private DefaultMultiHandler<PluginObject> _unloadHandlers;
        private Dictionary<string, ITAResponseEventHandler> _responseHandlers;
        private class Nested
        {
            // 显式静态构造告诉C＃编译器未标记类型BeforeFieldInit
            // 保证在调用Nested静态类时才进行实例初始化
            static Nested() { }
            internal static readonly TAEventDispatcher Instance = new TAEventDispatcher();
        }
        private TAEventDispatcher()
        {
            _handlers = new Dictionary<string, ITAEventHandler>();
            _responseHandlers = new Dictionary<string, ITAResponseEventHandler>();
        }
        /// <summary>
        /// 事件分发扩展
        /// </summary>
        /// <param name="scope"></param>
        public void AddScope(IDispatcher scope)
        {
            _scopelist.Add(scope);
        }
        public bool RemoveScope(IDispatcher scope)
        {
            return _scopelist.Remove(scope);
        }

        public static TAEventDispatcher Instance
        {
            get
            {
                return Nested.Instance;
            }
        }

        public override void Register(string key, ITAEventHandler handler)
        {
            _handlers[key] = handler;
        }

        public override void RegisterReponse(string key, ITAResponseEventHandler handler)
        {
            _responseHandlers[key] = handler;
        }

        /// <summary>
        /// 监听应用加载前初始化事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ac"></param>
        public void RegisterLoadBefore<T>(Action<T> ac) where T : TAAbstractApplication
        {
            Register(BEFORE_EVENT, new DefaultHandler<T>((p) =>
            {
                ac(p);
                return Task.CompletedTask;
            }));
        }

        /// <summary>
        /// 监听插件加载事件（可多次监听）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ac"></param>
        public void RegisterPluginLoad(Action<PluginObject> ac)
        {
            if (_loadHandlers == null)
            {
                _loadHandlers = new DefaultMultiHandler<PluginObject>();
                _loadHandlers.Register((p) =>
                {
                    ac(p);
                    return Task.CompletedTask;
                });
                Register(PLUGIN_LOAD_EVENT, _loadHandlers);
            }
            else
            {
                _loadHandlers.Register((p) =>
                {
                    ac(p);
                    return Task.CompletedTask;
                });
            }
        }
        /// <summary>
        /// 监听所有插件加载完成事件（可多次监听）
        /// </summary>
        /// <param name="ac"></param>
        public void RegisterPluginAllLoad(Func<string, Task> ac)
        {
            if (_allLoadHandlers == null)
            {
                _allLoadHandlers = new DefaultMultiHandler<string>();
                _allLoadHandlers.Register(ac);
                Register(PLUGIN_ALL_LOAD_EVENT, _allLoadHandlers);
            }
            else
            {
                _allLoadHandlers.Register(ac);
            }
        }
        /// <summary>
        /// 监听插件卸载事件（可多次监听）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ac"></param>
        public void RegisterPluginUnload(Action<PluginObject> ac)
        {
            if (_unloadHandlers == null)
            {
                _unloadHandlers = new DefaultMultiHandler<PluginObject>();
                _unloadHandlers.Register((p) =>
                {
                    ac(p);
                    return Task.CompletedTask;
                });
                Register(PLUGIN_UNLOAD_EVENT, _unloadHandlers);
            }
            else
            {
                _unloadHandlers.Register((p) =>
                {
                    ac(p);
                    return Task.CompletedTask;
                });
            }

        }

        /// <summary>
        /// 分发插件加载事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="plugin"></param>
        internal void DispathPluginLoad(PluginObject plugin)
        {
            TAAsyncHelper.RunSync(async () =>
            {
                await Dispatch(PLUGIN_LOAD_EVENT, plugin).ConfigureAwait(false);
            });
        }
        /// <summary>
        /// 分发所有插件加载完成事件
        /// </summary>
        internal void DispathPluginAllLoad()
        {
            TAAsyncHelper.RunSync(async () =>
            {
                await Dispatch(PLUGIN_ALL_LOAD_EVENT, string.Empty).ConfigureAwait(false);
            });
        }
        /// <summary>
        /// 分发插件卸载事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="plugin"></param>
        internal void DispathPluginUnload(PluginObject plugin)
        {
            TAAsyncHelper.RunSync(async () =>
            {
                await Dispatch(PLUGIN_UNLOAD_EVENT, plugin).ConfigureAwait(false);
            });
        }

        /// <summary>
        /// 分发应用加载前初始化事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="app"></param>
        internal void DispathLoadBefore<T>(T app) where T : TAAbstractApplication
        {
            TAAsyncHelper.RunSync(async () =>
            {
                await Dispatch(BEFORE_EVENT, app).ConfigureAwait(false);
            });

        }

        public async Task Dispatch<T>(T evt) where T : class
        {
            await Dispatch(typeof(T).ToString(), evt);
        }
        /// <summary>
        /// 分发事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="evt"></param>
        public override async Task Dispatch<T>(string key, T evt)
        {
            ITAEventHandler rt;
            if (_handlers.TryGetValue(key, out rt))
            {
                await rt.OnEventAsync(evt);
            }
            for (int i = 0; i < _scopelist.Count; i++)
            {
                await _scopelist[i].Dispatch(key, evt);
            }
        }
        public override async Task<Z> DispathWait<T, Z>(string key, T evt)
        {
            ITAResponseEventHandler rt;
            if (_responseHandlers.TryGetValue(key, out rt))
            {
                return await rt.OnEventWaitAsync<T, Z>(evt);
            }
            for (int i = 0; i < _scopelist.Count; i++)
            {
                var rsp = await _scopelist[i].DispathWait<T, Z>(key, evt);
                if (rsp != null && rsp.IsDone)
                {
                    return rsp;
                }
            }
            return null;
        }

    }
}
