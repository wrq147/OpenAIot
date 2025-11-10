using System;
using System.Threading.Tasks;
using TemplateAction.Label;

namespace TemplateAction.Core
{
    public interface IFilterMiddleware
    {
        Task<IResult> Excute(TAAction ac, FilterMiddlewareNode next);
    }
}
