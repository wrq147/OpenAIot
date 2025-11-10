using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using TemplateAction.Core;

namespace TemplateAction.Route
{
    public class Router : IRouter
    {
        protected static IDictionary<string, Type> _inlineConstraintMap = GetDefaultConstraintMap();
        protected static IDictionary<string, Type> GetDefaultConstraintMap()
        {
            return new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
            {
                // Type-specific constraints
                { "int", typeof(IntRouteConstraint) },
                { "bool", typeof(BoolRouteConstraint) },
                { "datetime", typeof(DateTimeRouteConstraint) },
                { "decimal", typeof(DecimalRouteConstraint) },
                { "double", typeof(DoubleRouteConstraint) },
                { "float", typeof(FloatRouteConstraint) },
                { "guid", typeof(GuidRouteConstraint) },
                { "long", typeof(LongRouteConstraint) },

                // Length constraints
                { "minlength", typeof(MinLengthRouteConstraint) },
                { "maxlength", typeof(MaxLengthRouteConstraint) },
                { "length", typeof(LengthRouteConstraint) },

                // Min/Max value constraints
                { "min", typeof(MinRouteConstraint) },
                { "max", typeof(MaxRouteConstraint) },
                { "range", typeof(RangeRouteConstraint) },

                // Regex-based constraints
                { "alpha", typeof(AlphaRouteConstraint) },
                { "regex", typeof(RegexInlineRouteConstraint) },

                {"required", typeof(RequiredRouteConstraint) },
                {"exists",typeof(ExistsRouteConstraint) }
            };
        }
        public RouteTemplate ParsedTemplate { get; protected set; }
        public IDictionary<string, IRouteConstraint> Constraints { get; protected set; }
        public IDictionary<string, object> Defaults { get; protected set; }
        private RouteTemplateMatcher _matcher;
        public Router(string template, IDictionary<string, object> defaults = null, IDictionary<string, IRouteConstraint> constraints = null)
        {
            ParsedTemplate = RouteTemplateParser.Parse(template);
            Constraints = GetConstraints(ParsedTemplate);
            Defaults = GetDefaults(ParsedTemplate, defaults);
            if (constraints != null)
            {
                foreach (KeyValuePair<string, IRouteConstraint> kvp in constraints)
                {
                    Constraints.Add(kvp.Key, kvp.Value);
                }
            }
        }
        public Router(string template, object defaults, object constraints = null)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            PropertyInfo[] props = defaults.GetType().GetProperties();
            foreach (PropertyInfo p in props)
            {
                dic.Add(p.Name, p.GetValue(defaults, null));
            }
            ParsedTemplate = RouteTemplateParser.Parse(template);
            Constraints = GetConstraints(ParsedTemplate);
            Defaults = GetDefaults(ParsedTemplate, dic);

            if (constraints != null)
            {
                Dictionary<string, IRouteConstraint> constdic = new Dictionary<string, IRouteConstraint>();
                PropertyInfo[] constprops = defaults.GetType().GetProperties();
                foreach (PropertyInfo p in constprops)
                {
                    constdic.Add(p.Name, (IRouteConstraint)p.GetValue(constraints, null));
                }
                foreach (KeyValuePair<string, IRouteConstraint> kvp in constdic)
                {
                    Constraints.Add(kvp.Key, kvp.Value);
                }
            }
        
        }
        /// <summary>
        /// 获取默认值
        /// </summary>
        /// <param name="parsedTemplate"></param>
        /// <returns></returns>
        protected static IDictionary<string, object> GetDefaults(RouteTemplate parsedTemplate, IDictionary<string, object> defaults)
        {
            IDictionary<string, object> result = defaults == null ? new Dictionary<string, object>() : defaults;
            foreach (RouteTemplatePart parameter in parsedTemplate.Parameters)
            {
                if (parameter.DefaultValue != null)
                {
                    result.Add(parameter.Name, parameter.DefaultValue);
                }
            }

            return result;
        }
        /// <summary>
        /// 获取约束
        /// </summary>
        /// <param name="parsedTemplate"></param>
        /// <returns></returns>
        protected static IDictionary<string, IRouteConstraint> GetConstraints(RouteTemplate parsedTemplate)
        {
            Dictionary<string, List<IRouteConstraint>> constraints = new Dictionary<string, List<IRouteConstraint>>(StringComparer.OrdinalIgnoreCase);
            HashSet<string> optionalParameters = new HashSet<string>();
            foreach (var parameter in parsedTemplate.Parameters)
            {
                if (parameter.IsOptional)
                {
                    optionalParameters.Add(parameter.Name);
                }

                foreach (RouteInlineConstraint inlineConstraint in parameter.InlineConstraints)
                {
                    IRouteConstraint constraint = ResolveConstraint(inlineConstraint.Constraint);
                    List<IRouteConstraint> list;
                    if (!constraints.TryGetValue(parameter.Name, out list))
                    {
                        list = new List<IRouteConstraint>();
                        constraints.Add(parameter.Name, list);
                    }
                    list.Add(constraint);
                }
            }


            Dictionary<string, IRouteConstraint> newconstraints = new Dictionary<string, IRouteConstraint>(StringComparer.OrdinalIgnoreCase);
            foreach (var kvp in constraints)
            {
                IRouteConstraint constraint;
                if (kvp.Value.Count == 1)
                {
                    constraint = kvp.Value[0];
                }
                else
                {
                    constraint = new CompositeRouteConstraint(kvp.Value.ToArray());
                }

                if (optionalParameters.Contains(kvp.Key))
                {
                    var optionalConstraint = new OptionalRouteConstraint(constraint);
                    newconstraints.Add(kvp.Key, optionalConstraint);
                }
                else
                {
                    newconstraints.Add(kvp.Key, constraint);
                }
            }
            return newconstraints;
        }
        /// <summary>
        /// 解释约束
        /// </summary>
        /// <param name="inlineConstraint"></param>
        /// <returns></returns>
        protected static IRouteConstraint ResolveConstraint(string inlineConstraint)
        {
            if (inlineConstraint == null)
            {
                throw new ArgumentNullException(nameof(inlineConstraint));
            }
            string constraintKey;
            string argumentString;
            var indexOfFirstOpenParens = inlineConstraint.IndexOf('(');
            if (indexOfFirstOpenParens >= 0 && inlineConstraint.EndsWith(")", StringComparison.Ordinal))
            {
                constraintKey = inlineConstraint.Substring(0, indexOfFirstOpenParens);
                argumentString = inlineConstraint.Substring(indexOfFirstOpenParens + 1,
                                                            inlineConstraint.Length - indexOfFirstOpenParens - 2);
            }
            else
            {
                constraintKey = inlineConstraint;
                argumentString = null;
            }

            Type constraintType;
            if (!_inlineConstraintMap.TryGetValue(constraintKey, out constraintType))
            {
                return null;
            }
            return (IRouteConstraint)Activator.CreateInstance(constraintType);
        }
        protected static IRouteConstraint CreateConstraint(Type constraintType, string argumentString)
        {
            // No arguments - call the default constructor
            if (argumentString == null)
            {
                return (IRouteConstraint)Activator.CreateInstance(constraintType);
            }

            ConstructorInfo activationConstructor = null;
            object[] parameters = null;
            ConstructorInfo[] constructors = constraintType.GetConstructors(BindingFlags.Instance | BindingFlags.Public);

            // If there is only one constructor and it has a single parameter, pass the argument string directly
            // This is necessary for the Regex RouteConstraint to ensure that patterns are not split on commas.
            if (constructors.Length == 1 && constructors[0].GetParameters().Length == 1)
            {
                activationConstructor = constructors[0];
                parameters = ConvertArguments(activationConstructor.GetParameters(), new string[] { argumentString });
            }
            else
            {
                string[] arguments = argumentString.Split(',').Select(argument => argument.Trim()).ToArray();
                ConstructorInfo[] matchingConstructors = constructors.Where(ci => ci.GetParameters().Length == arguments.Length)
                                                       .ToArray();
                activationConstructor = matchingConstructors[0];
                parameters = ConvertArguments(activationConstructor.GetParameters(), arguments);
            }

            return (IRouteConstraint)activationConstructor.Invoke(parameters);
        }
        protected static object[] ConvertArguments(ParameterInfo[] parameterInfos, string[] arguments)
        {
            var parameters = new object[parameterInfos.Length];
            for (var i = 0; i < parameterInfos.Length; i++)
            {
                var parameter = parameterInfos[i];
                var parameterType = parameter.ParameterType;
                parameters[i] = Convert.ChangeType(arguments[i], parameterType);
            }

            return parameters;
        }
        protected bool Match(
           IDictionary<string, IRouteConstraint> constraints,
           IDictionary<string, object> routeValues,
           ITAContext context)
        {
            if (routeValues == null)
            {
                throw new ArgumentNullException(nameof(routeValues));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }


            if (constraints == null || constraints.Count == 0)
            {
                return true;
            }

            foreach (var kvp in constraints)
            {
                var constraint = kvp.Value;
                if (!constraint.Match(context, this, kvp.Key, routeValues))
                {
                    return false;
                }
            }

            return true;
        }
        private const string ROUTE_DATA_KEY = "$ROUTE_DATA";
        public IDictionary<string, object> Route(ITAContext context)
        {
            if (_matcher == null)
            {
                _matcher = new RouteTemplateMatcher(ParsedTemplate, Defaults);
            }
            Dictionary<string, object> dict = context.Items[ROUTE_DATA_KEY] as Dictionary<string, object>;
            if (dict == null)
            {
                dict = new Dictionary<string, object>();
                context.Items[ROUTE_DATA_KEY] = dict;
            }
            else
            {
                dict.Clear();
            }
            if (!_matcher.TryMatch(context.Request.Path, dict))
            {
                return null;
            }
            if (!Match(Constraints, dict, context))
            {
                return null;
            }
            return dict;
        }
    }
}
