using System;
using System.Collections.Generic;
using TemplateAction.Common;
using TemplateAction.Core;

namespace TemplateAction.Route
{
    public class ExistsRouteConstraint : IRouteConstraint
    {
        public bool Match(ITAContext context, IRouter route, string routeKey, IDictionary<string, object> values)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (route == null)
            {
                throw new ArgumentNullException(nameof(route));
            }

            if (routeKey == null)
            {
                throw new ArgumentNullException(nameof(routeKey));
            }

            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }
            object value;
            if (values.TryGetValue(routeKey, out value) && value != null)
            {
                string valueStr = value as string;
                if (valueStr != null)
                {
                    switch (routeKey)
                    {
                        case TAUtility.NS_KEY:
                            return context.Application.ExistPlugin(valueStr.ToLower());
                        case TAUtility.CONTROLLER_KEY:
                            return context.Application.ExistController(valueStr.ToLower());
                    }
                }
            }

            return false;
        }
    }
}
