using System.Collections.Generic;
using TemplateAction.Core;

namespace TemplateAction.Route
{
    public interface IRouteConstraint
    {
        bool Match(ITAContext context,IRouter route,string routeKey,IDictionary<string,object> values);
    }
}
