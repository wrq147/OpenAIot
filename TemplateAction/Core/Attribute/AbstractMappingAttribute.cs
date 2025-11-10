using System;
using System.Reflection;
using System.Threading.Tasks;

namespace TemplateAction.Core
{
    public abstract class AbstractMappingAttribute : Attribute
    {
        public abstract Task<object> Mapping(TAAction ac, string key, Type t);
    }
}
