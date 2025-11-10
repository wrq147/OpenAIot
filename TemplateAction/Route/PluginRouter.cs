using System;
using System.Collections.Generic;
using TemplateAction.Core;

namespace TemplateAction.Route
{
    /// <summary>
    /// 插件里定义的路由
    /// </summary>
    public class PluginRouter : Router, IPluginRouter
    {
        private int _sortIndex;
        public int SortIndex
        {
            get { return _sortIndex; }
        }
        public PluginRouter(string template, int sortIndex, IDictionary<string, object> defaults = null, IDictionary<string, IRouteConstraint> constraints = null) : base(template, defaults, constraints)
        {
            _sortIndex = sortIndex;
        }
    }
}
