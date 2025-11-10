using System;
using System.Collections.Generic;
using TemplateAction.Core;

namespace TemplateAction.Route
{
    public class DeleteConstraint : IRouteConstraint
    {
        public bool Match(ITAContext context, IRouter route, string routeKey, IDictionary<string, object> values)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            return context.Request.HttpMethod.ToLower() == "delete";
        }
    }
}
