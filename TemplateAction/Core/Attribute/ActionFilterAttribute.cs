using System;
using System.Threading.Tasks;
using TemplateAction.Label;

namespace TemplateAction.Core
{
    /// <summary>
    /// 注解过滤器
    /// </summary>
    public abstract class ActionFilterAttribute : NodeAttribute, IFilterMiddleware
    {
        public override void Config(PluginObject plg, ControllerNode controller, ActionNode node) { }
        public abstract Task<IResult> Excute(TAAction ac, FilterMiddlewareNode next);
    }
}
