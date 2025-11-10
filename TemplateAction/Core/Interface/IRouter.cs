using System.Collections.Generic;

namespace TemplateAction.Core
{
    public interface IRouter
    {
        IDictionary<string, object> Route(ITAContext context);
    }
}
