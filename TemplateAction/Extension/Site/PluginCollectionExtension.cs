using System;
using TemplateAction.Core;
using System.Collections.Generic;

namespace TemplateAction.Extension.Site
{
    public static class PluginCollectionExtension
    {
        public static PluginCollection UseRouters(this PluginCollection collection, IRouterCollection<IRouter> routers)
        {
            SitePluginCollectionExtData sitefactory = (SitePluginCollectionExtData)collection.ExtentionData;
            sitefactory.RouterCollection = routers;
            return collection;
        }
        /// <summary>
        /// 路由
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static IDictionary<string, object> Route(this PluginCollection collection, ITAContext context)
        {
            SitePluginCollectionExtData sitefactory = (SitePluginCollectionExtData)collection.ExtentionData;
            IDictionary<string, object> rt = null;

            //先路由插件
            PluginObject[] tarr = collection.GetAllPlugin();
            foreach (PluginObject plg in tarr)
            {
                rt = plg.Route(context);
                if (rt != null)
                {
                    return rt;
                }
            }
            //再路由全局
            if (sitefactory.RouterCollection != null)
            {
                rt = sitefactory.RouterCollection.Route(context);
                if (rt != null)
                {
                    return rt;
                }
            }

            return rt;
        }
        /// <summary>
        /// 获取控制器和动作的描述信息
        /// </summary>
        /// <returns></returns>
        public static List<DescribeInfo> FindAllDescribe(this PluginCollection collection)
        {
            PluginObject[] tarr = collection.GetAllPlugin();

            List<DescribeInfo> rtlist = new List<DescribeInfo>();
            foreach (PluginObject plg in tarr)
            {
                rtlist.AddRange(plg.FindDescribes());
            }
            return rtlist;
        }



        /// <summary>
        /// 获取Controller节点
        /// </summary>
        /// <param name="ns"></param>
        /// <param name="controller"></param>
        /// <returns></returns>
        public static ControllerNode GetControllerByKeyInPlugin(this PluginCollection collection, string ns, string controller)
        {
            PluginObject pobj = collection.GetPlugin(ns);
            if (pobj != null)
            {
                return pobj.GetControllerNodeByKey(controller);
            }
            return null;
        }

        /// <summary>
        /// 获取Action节点
        /// </summary>
        /// <param name="ns"></param>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static ActionNode GetMethodByKeyInPlugin(this PluginCollection collection, string ns, string controller, string action)
        {
            PluginObject pobj = collection.GetPlugin(ns);
            if (pobj != null)
            {
                return pobj.GetMethodByKey(controller, action);
            }
            return null;
        }
        public static bool ExistController(this PluginCollection collection, string key)
        {
            PluginObject[] tarr = collection.GetAllPlugin();
            foreach (PluginObject plg in tarr)
            {
                if (plg.ContainController(key))
                {
                    return true;
                }
            }
            return false;
        }

    }
}
