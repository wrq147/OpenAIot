using System;

namespace TemplateAction.Core
{
    /// <summary>
    /// 配置控制器用
    /// </summary>
    public abstract class NodeControllerAttribute: NodeAttribute
    {
        public override void Config(PluginObject plg, ControllerNode controller, ActionNode node)
        {
            if (node == null)
            {
                if (controller.IsConfiged)
                {
                    ConfigAfterAction(plg, controller);
                }
                else
                {
                    ConfigBeforeAction(plg, controller);
                }
            }
            else
            {
                ConfigAction(plg, controller, node);
            }
        }
        public abstract void ConfigAction(PluginObject plg, ControllerNode controller, ActionNode node);
        public abstract void ConfigBeforeAction(PluginObject plg, ControllerNode controller);
        public abstract void ConfigAfterAction(PluginObject plg, ControllerNode controller);
    }
}
