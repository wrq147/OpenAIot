using System;

namespace TemplateAction.Core
{
    public interface ITAFormCollection : ITAObjectCollection
    {
        IRequestFile[] Files { get; }

    }
}
