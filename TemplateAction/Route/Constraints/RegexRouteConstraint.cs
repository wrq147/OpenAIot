using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TemplateAction.Core;

namespace TemplateAction.Route
{
    public class RegexRouteConstraint : IRouteConstraint
    {
        public RegexRouteConstraint(Regex regex)
        {
            if (regex == null)
            {
                throw new ArgumentNullException(nameof(regex));
            }

            Constraint = regex;
        }

        public RegexRouteConstraint(string regexPattern)
        {
            if (regexPattern == null)
            {
                throw new ArgumentNullException(nameof(regexPattern));
            }

            Constraint = new Regex(regexPattern);
        }

        public Regex Constraint { get; private set; }

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

            object routeValue;

            if (values.TryGetValue(routeKey, out routeValue)
                && routeValue != null)
            {
                var parameterValueString = Convert.ToString(routeValue);

                return Constraint.IsMatch(parameterValueString);
            }

            return false;
        }
    }
}
