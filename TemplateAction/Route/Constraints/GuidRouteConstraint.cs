using System;
using System.Collections.Generic;
using TemplateAction.Core;

namespace TemplateAction.Route
{
    /// <summary>
    /// Constrains a route parameter to represent only <see cref="Guid"/> values.
    /// Matches values specified in any of the five formats "N", "D", "B", "P", or "X",
    /// supported by Guid.ToString(string) and Guid.ToString(String, IFormatProvider) methods.
    /// </summary>
    public class GuidRouteConstraint : IRouteConstraint
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
                if (value is Guid)
                {
                    return true;
                }

                Guid result;
                var valueString = Convert.ToString(value);
                return Guid.TryParse(valueString, out result);
            }

            return false;
        }
    }
}
