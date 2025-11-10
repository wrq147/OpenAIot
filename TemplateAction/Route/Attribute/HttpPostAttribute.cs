using System;
using TemplateAction.Core;

namespace TemplateAction.Route
{
    /// <summary>
    /// 标记Action为Post的HttpMethod
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class HttpPostAttribute : NodeAttribute
    {
        public override void Config(PluginObject plg, ControllerNode controller, ActionNode node)
        {
            node.AddHttpMethod(NodeHttpMethod.Post);
        }
    }
}
