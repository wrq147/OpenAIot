using System;
using System.Collections.Generic;
using TemplateAction.Common;
using TemplateAction.Core;

namespace TemplateAction.Route
{
    /// <summary>
    /// 通用路由注解,建议只在特定的地址上使用
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class RouteAttribute : NodeControllerAttribute
    {
        private string _template;
        public RouteAttribute(string template = null)
        {
            _template = template;
        }

        public override void ConfigBeforeAction(PluginObject plg, ControllerNode controller)
        {
            //控制器配置
            RoutePrefixList list = controller.GetExtra<RoutePrefixList>();
            if (list == null)
            {
                list = new RoutePrefixList();
            }
            list.Add(_template);
            controller.SetExtra<RoutePrefixList>(list);
            IDictionary<string, object> defaults = new Dictionary<string, object>();
            defaults.Add(TAUtility.CONTROLLER_KEY, controller.Key);
            plg.Data.Get<PluginRouterCollection>().Add(new PluginRouter(_template, 1, defaults));
        }
        public override void ConfigAction(PluginObject plg, ControllerNode controller, ActionNode node)
        {
            //节点配置
            IDictionary<string, object> defaults = new Dictionary<string, object>();
            defaults.Add(TAUtility.CONTROLLER_KEY, controller.Key);
            defaults.Add(TAUtility.ACTION_KEY, node.Key);
            RoutePrefixList list = controller.GetExtra<RoutePrefixList>();
            if (list == null)
            {
                plg.Data.Get<PluginRouterCollection>()?.Add(new PluginRouter(_template, 0, defaults));
            }
            else
            {
                PluginRouterCollection routers = plg.Data.Get<PluginRouterCollection>();
                if (list.Count == 0)
                {
                    routers.Add(new PluginRouter(_template, 0, defaults));
                }
                else
                {
                    string tmpStr = _template;
                    if (!tmpStr.StartsWith("/"))
                    {
                        tmpStr = "/" + tmpStr;
                    }
                    foreach (string prefix in list)
                    {
                        string tmpPrefix = prefix;
                        if (tmpPrefix.EndsWith("/"))
                        {
                            tmpPrefix = tmpPrefix.Substring(0, tmpPrefix.Length - 1);
                        }
                        routers?.Add(new PluginRouter(tmpPrefix + tmpStr, 0, defaults));
                    }
                }
            }
        }
        public override void ConfigAfterAction(PluginObject plg, ControllerNode controller)
        {
        }

    }
}
