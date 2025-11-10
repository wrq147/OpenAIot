using System;

namespace TemplateAction.Core
{
    public class SitePluginExtDataFactory : IPluginExtDataFactory
    {
        public IPluginExtData CreateExtData()
        {
            return new SitePluginCollectionExtData();
        }
    }
}
