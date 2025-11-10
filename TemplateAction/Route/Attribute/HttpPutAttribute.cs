using System;
using TemplateAction.Core;

namespace TemplateAction.Route
{
    /// <summary>
    /// 标记Action为Put的HttpMethod
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class HttpPutAttribute : NodeAttribute
    {
        public override void Config(PluginObject plg, ControllerNode controller, ActionNode node)
        {
            node.AddHttpMethod(NodeHttpMethod.Put);
        }

    }
}
