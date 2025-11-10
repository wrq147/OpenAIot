using System;
using TemplateAction.Core;

namespace TemplateAction.Route
{
    /// <summary>
    /// 标记Action为Delete的HttpMethod
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class HttpDeleteAttribute : NodeAttribute
    {
        public override void Config(PluginObject plg, ControllerNode controller, ActionNode node)
        {
            node.AddHttpMethod(NodeHttpMethod.Delete);
        }
    }
}
