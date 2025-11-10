using System;
using System.Collections.Generic;
using TemplateAction.Core;

namespace TemplateAction.Route
{
    /// <summary>
    /// 只接收Put
    /// </summary>
    public class PutConstraint : IRouteConstraint
    {
        public bool Match(ITAContext context, IRouter route, string routeKey, IDictionary<string, object> values)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            return context.Request.HttpMethod.ToLower() == "put";
        }
    }
}
