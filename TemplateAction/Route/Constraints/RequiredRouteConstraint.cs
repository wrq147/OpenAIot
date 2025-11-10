using System;
using System.Collections.Generic;
using TemplateAction.Core;

namespace TemplateAction.Route
{
    /// <summary>
    /// Constraints a route parameter that must have a value.
    /// </summary>
    /// <remarks>
    /// This constraint is primarily used to enforce that a non-parameter value is present during
    /// URL generation.
    /// </remarks>
    public class RequiredRouteConstraint : IRouteConstraint
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
                // In routing the empty string is equivalent to null, which is equivalent to an unset value.
                var valueString = Convert.ToString(value);
                return !string.IsNullOrEmpty(valueString);
            }

            return false;
        }
    }
}
