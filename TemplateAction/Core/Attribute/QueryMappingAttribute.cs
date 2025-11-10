using System;
using System.Threading.Tasks;

namespace TemplateAction.Core
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class QueryMappingAttribute : AbstractMappingAttribute
    {
        public override Task<object> Mapping(TAAction ac, string key, Type t)
        {
            return Task.FromResult(DefatulParamMapping.TAObjectMapping(ac.Context.Request.Query, ac, key, t));
        }
    }
}
