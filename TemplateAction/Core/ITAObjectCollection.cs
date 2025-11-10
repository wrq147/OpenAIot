using System;
using System.Collections;
using System.Reflection;

namespace TemplateAction.Core
{
    public interface ITAObjectCollection : IEnumerable
    {
        object this[string key] { get; }
        bool TryGet(string key, out object result);
        int Count { get; }
    }
}
