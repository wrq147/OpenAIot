using System;

namespace TemplateAction.Core
{
    public class DefaultLoaderFactory : ILoaderFactory
    {
        public IPluginLoader CreateLoader(PluginCollection collection)
        {
            return new PluginLoader(collection);
        }
    }
}
