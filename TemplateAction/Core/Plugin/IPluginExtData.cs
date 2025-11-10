
using System;
using System.Reflection;

namespace TemplateAction.Core
{
    public interface IPluginExtData
    {
        void PluginLoadBefore(PluginObject plg);
        bool PluginLoadType(PluginObject plg, Type t);
        void PluginLoadAfter(PluginObject plg);
    }
}
