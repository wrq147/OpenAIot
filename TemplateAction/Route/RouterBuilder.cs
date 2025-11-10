using System;
using System.Collections.Generic;
using TemplateAction.Common;
using TemplateAction.Core;

namespace TemplateAction.Route
{
    public class RouterBuilder
    {
        protected IRouterCollection<IRouter> _tmplist = new RouterCollection();
        /// <summary>
        /// 使用WebApi默认路由
        /// </summary>
        /// <param name="ns">默认命名空间</param>
        /// <returns></returns>
        public RouterBuilder UseDefault(string ns)
        {
            Dictionary<string, object> defaults = new Dictionary<string, object>();
            defaults.Add(TAUtility.NS_KEY, ns);
            defaults.Add(TAUtility.CONTROLLER_KEY, "Home");
            defaults.Add(TAUtility.ACTION_KEY, "Index");
            string template = string.Format("{{{0}}}/{{{1}}}/{{id?}}", TAUtility.CONTROLLER_KEY, TAUtility.ACTION_KEY);

            return MapRoute(new Router(template, defaults));
        }
        /// <summary>
        /// 使用WebApi插件路由
        /// </summary>
        /// <returns></returns>
        public RouterBuilder UsePlugin()
        {
            Dictionary<string, object> defaults = new Dictionary<string, object>();
            defaults.Add(TAUtility.CONTROLLER_KEY, "Home");
            defaults.Add(TAUtility.ACTION_KEY, "Index");
            string template = string.Format("{{{0}:exists}}/{{{1}}}/{{{2}}}/{{id?}}", TAUtility.NS_KEY, TAUtility.CONTROLLER_KEY, TAUtility.ACTION_KEY);

            return MapRoute(new Router(template, defaults));
        }
        /// <summary>
        /// 使用Restful接口
        /// </summary>
        /// <param name="ns">默认命名空间</param>
        /// <returns></returns>
        public RouterBuilder UseRestful(string ns)
        {
            string template = string.Format("{{{0}}}/{{id?}}", TAUtility.CONTROLLER_KEY);
            Dictionary<string, object> getdefaults = new Dictionary<string, object>();
            getdefaults.Add(TAUtility.NS_KEY, ns);
            MapRoute(new Router(template, getdefaults));
            return this;
        }

        /// <summary>
        /// 使用Restful插件路由
        /// </summary>
        /// <param name="ns"></param>
        /// <returns></returns>
        public RouterBuilder UseRestfulPlugin()
        {
            string template = string.Format("{{{0}:exists}}/{{{1}}}/{{id?}}", TAUtility.NS_KEY, TAUtility.CONTROLLER_KEY);
            MapRoute(new Router(template, null));
            return this;
        }
        /// <summary>
        /// 添加路由
        /// </summary>
        /// <param name="router"></param>
        /// <returns></returns>
        public RouterBuilder MapRoute(IRouter router)
        {
            _tmplist.Add(router);
            return this;
        }
        /// <summary>
        /// 添加指定模块映射路由
        /// </summary>
        /// <param name="ns"></param>
        /// <param name="template"></param>
        /// <returns></returns>
        public RouterBuilder MapRoute(string ns, string template)
        {
            Dictionary<string, object> getdefaults = new Dictionary<string, object>();
            getdefaults.Add(TAUtility.NS_KEY, ns);
            MapRoute(new Router(template, getdefaults));
            return this;
        }


        public IRouterCollection<IRouter> Build()
        {
            return _tmplist;
        }
    }
}
