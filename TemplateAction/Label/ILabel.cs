using System;

namespace TemplateAction.Label
{
    public interface ILabel
    {
        TAParams Param { get; }
        ILabel Parent { get;}
    }
}
