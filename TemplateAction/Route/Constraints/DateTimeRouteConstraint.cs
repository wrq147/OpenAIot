using System;
using System.Collections.Generic;
using TemplateAction.Core;

namespace TemplateAction.Route
{
    /// <summary>
    /// Constrains a route parameter to represent only <see cref="DateTime"/> values.
    /// </summary>
    /// <remarks>
    /// This constraint tries to parse strings by using all of the formats returned by the
    /// CultureInfo.InvariantCulture.DateTimeFormat.GetAllDateTimePatterns() method.
    /// For a sample on how to list all formats which are considered, please visit
    /// http://msdn.microsoft.com/en-us/library/aszyst2c(v=vs.110).aspx
    /// </remarks>
    public class DateTimeRouteConstraint : IRouteConstraint
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
                if (value is DateTime)
                {
                    return true;
                }

                DateTime result;
                var valueString = Convert.ToString(value);
                return DateTime.TryParse(valueString, out result);
            }

            return false;
        }
    }
}
