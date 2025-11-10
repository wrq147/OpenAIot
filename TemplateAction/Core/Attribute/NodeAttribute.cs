using System;

namespace TemplateAction.Core
{
    public abstract class NodeAttribute : Attribute
    {
        public abstract void Config(PluginObject plg, ControllerNode controller, ActionNode node);
    }
}
