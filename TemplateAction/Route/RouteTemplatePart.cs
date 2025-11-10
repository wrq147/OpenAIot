using System;
using System.Collections.Generic;

namespace TemplateAction.Route
{
    public class RouteTemplatePart
    {
        public static RouteTemplatePart CreateLiteral(string text)
        {
            return new RouteTemplatePart()
            {
                IsLiteral = true,
                Text = text,
            };
        }

        public static RouteTemplatePart CreateParameter(string name,
                                                   bool isCatchAll,
                                                   bool isOptional,
                                                   object defaultValue,
                                                   IEnumerable<RouteInlineConstraint> inlineConstraints)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            return new RouteTemplatePart()
            {
                IsParameter = true,
                Name = name,
                IsCatchAll = isCatchAll,
                IsOptional = isOptional,
                DefaultValue = defaultValue,
                InlineConstraints = inlineConstraints,
            };
        }

        public bool IsCatchAll { get; private set; }
        public bool IsLiteral { get; private set; }
        public bool IsParameter { get; private set; }
        public bool IsOptional { get; private set; }
        public bool IsOptionalSeperator { get; set; }
        public string Name { get; private set; }
        public string Text { get; private set; }
        public object DefaultValue { get; private set; }
        public IEnumerable<RouteInlineConstraint> InlineConstraints { get; private set; }

    }
}
