using System;
namespace TemplateAction.Core
{
    public interface IPluginRouter : IRouter
    {
        int SortIndex { get; }
    }
}
