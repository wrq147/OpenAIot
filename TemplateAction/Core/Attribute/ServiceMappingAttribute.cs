using System;
using System.Threading.Tasks;
using TemplateAction.Label;

namespace TemplateAction.Core
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class ServiceMappingAttribute : AbstractMappingAttribute
    {
        public override Task<object> Mapping(TAAction ac, string key, Type t)
        {
            object result = ac.Context.Application.ServiceProvider.GetService(t);
            if (result == null)
            {
                return Task.FromResult<object>(DBNull.Value);
            }
            return Task.FromResult(result);
        }
    }
}
