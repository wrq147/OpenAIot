using System;
using System.Threading.Tasks;

namespace TemplateAction.Core
{
    /// <summary>
    /// 绑定form
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class FormMappingAttribute : AbstractMappingAttribute
    {
        public override async Task<object> Mapping(TAAction ac, string key, Type t)
        {
            return DefatulParamMapping.TAObjectMapping(await ac.Context.Request.ReadFormAsync(), ac, key, t);
        }
    }
}
