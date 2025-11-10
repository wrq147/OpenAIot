using System;
using System.Collections.Generic;
using TemplateAction.Core;

namespace TemplateAction.Route
{
    public class OptionalRouteConstraint : IRouteConstraint
    {
        public OptionalRouteConstraint(IRouteConstraint innerConstraint)
        {
            if (innerConstraint == null)
            {
                throw new ArgumentNullException(nameof(innerConstraint));
            }

            InnerConstraint = innerConstraint;
        }

        public IRouteConstraint InnerConstraint { get; }

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
            if (values.TryGetValue(routeKey, out value))
            {
                return InnerConstraint.Match(context,
                                             route,
                                             routeKey,
                                             values);
            }

            return true;
        }
    }
}
