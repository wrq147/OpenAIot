using System;
using System.Collections.Generic;
using TemplateAction.Core;

namespace TemplateAction.Route
{
    /// <summary>
    /// Constrains a route parameter to represent only Boolean values.
    /// </summary>
    public class BoolRouteConstraint : IRouteConstraint
    {
        /// <inheritdoc />
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
                if (value is bool)
                {
                    return true;
                }

                bool result;
                var valueString = Convert.ToString(value);
                return bool.TryParse(valueString, out result);
            }

            return false;
        }
    }
}
