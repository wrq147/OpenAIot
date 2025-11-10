using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace TemplateAction.Core
{
    public interface IParamMapping
    {
        Task<object> Mapping(LinkedListNode<IParamMapping> next,TAAction ac, string key, Type t);
    }
}
