using System;
using TemplateAction.Core;

namespace TemplateAction.Route
{
    /// <summary>
    /// 标记Action为Get的HttpMethod
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class HttpGetAttribute : NodeAttribute
    {
        public override void Config(PluginObject plg, ControllerNode controller, ActionNode node)
        {
            node.AddHttpMethod(NodeHttpMethod.Get);
        }
    }
}
