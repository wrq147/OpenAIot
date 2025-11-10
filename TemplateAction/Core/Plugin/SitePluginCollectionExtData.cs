using System;
using System.Collections.Generic;

namespace TemplateAction.Core
{
    public class SitePluginCollectionExtData : IPluginExtData
    {
        private static string ControllerInterfaceName = typeof(IController).FullName;
        internal IRouterCollection<IRouter> RouterCollection { get; set; }
        /// <summary>
        /// 初始化插件扩展数据
        /// </summary>
        /// <param name="plg"></param>
        public void PluginLoadBefore(PluginObject plg)
        {
            plg.Data.Set(new PluginRouterCollection());
            plg.Data.Set(new Dictionary<string, ControllerNode>());
        }
        public bool PluginLoadType(PluginObject plg, Type t)
        {
            if (t.GetInterface(ControllerInterfaceName) != null)
            {
                //获取插件的控制器节点数据
                ControllerNode n = new ControllerNode(plg, t);
                plg.Data.Get<Dictionary<string, ControllerNode>>()[n.Key.ToLower()] = n;
                return true;
            }
            return false;
        }
        public void PluginLoadAfter(PluginObject plg)
        {
        }
    }
}
