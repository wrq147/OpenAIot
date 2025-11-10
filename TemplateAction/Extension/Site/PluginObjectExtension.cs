using System;
using System.Collections.Generic;
using TemplateAction.Common;
using TemplateAction.Core;

namespace TemplateAction.Extension.Site
{
    public static class PluginObjectExtension
    {
        public static List<DescribeInfo> FindDescribes(this PluginObject obj)
        {
            List<DescribeInfo> rtlist = new List<DescribeInfo>();
            Dictionary<string, ControllerNode> ml = obj.GetControllerList();
            foreach (KeyValuePair<string, ControllerNode> kpcn in ml)
            {
                if (!string.IsNullOrEmpty(kpcn.Value.Descript))
                {
                    DescribeInfo ai = new DescribeInfo();
                    ai.Name = kpcn.Value.Descript;
                    ai.Sort = kpcn.Value.Sort;
                    ai.Code = string.Format("/{0}/{1}/", obj.Name, kpcn.Value.Key);
                    ai.ParentCode = string.Format("/{0}/", obj.Name);
                    rtlist.Add(ai);
                    foreach (KeyValuePair<string, Node> kpn in kpcn.Value.Childrens)
                    {
                        ActionNode an = kpn.Value as ActionNode;
                        if (an == null) continue;
                        if (!string.IsNullOrEmpty(kpn.Value.Descript))
                        {
                            DescribeInfo aii = new DescribeInfo();
                            aii.Name = kpn.Value.Descript;
                            aii.Sort = kpn.Value.Sort;
                            aii.Code = string.Format("/{0}/{1}/{2}", obj.Name, kpcn.Value.Key, kpn.Value.Key);
                            aii.ParentCode = ai.Code;
                            aii.AboutCode = an.AboutCode;
                            rtlist.Add(aii);
                        }
                    }
                }
            }
            return rtlist;
        }
        public static Dictionary<string, ControllerNode> GetControllerList(this PluginObject obj)
        {
            return obj.Data.Get<Dictionary<string, ControllerNode>>();
        }
        public static bool ContainController(this PluginObject obj, string key)
        {
            return obj.Data.Get<Dictionary<string, ControllerNode>>().ContainsKey(key);
        }
        public static ControllerNode GetControllerNodeByKey(this PluginObject obj, string controller)
        {
            ControllerNode rtVal = null;
            if (obj.Data.Get<Dictionary<string, ControllerNode>>().TryGetValue(controller.ToLower(), out rtVal))
            {
                return rtVal;
            }
            return null;
        }
        public static ActionNode GetMethodByKey(this PluginObject obj, string controller, string action)
        {
            ControllerNode rtVal = null;
            if (obj.Data.Get<Dictionary<string, ControllerNode>>().TryGetValue(controller.ToLower(), out rtVal))
            {
                ActionNode an = rtVal.GetChildNode(action) as ActionNode;
                if (an != null)
                {
                    return an;
                }
            }
            return null;
        }

        public static IDictionary<string, object> Route(this PluginObject obj, ITAContext context)
        {
            IDictionary<string, object> rt = obj.Data.Get<PluginRouterCollection>().Route(context);
            if (rt != null)
            {
                rt[TAUtility.NS_KEY] = obj.Name;
                return rt;
            }
            return null;
        }
    }
}
