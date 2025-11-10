using System;

namespace TemplateAction.Label
{
    public interface ITemplatePath
    {
        string NameSpace { get; }
        string Controller { get; }
        string Action { get; }
    }
}
