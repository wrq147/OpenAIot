using System;

using TemplateAction.Core;

namespace TemplateAction.NetCore
{
    public class TANetLoaderFactory : ILoaderFactory
    {
        public IPluginLoader CreateLoader(PluginCollection collection)
        {
            return new TANetCorePluginLoader(collection);
        }
    }
}
