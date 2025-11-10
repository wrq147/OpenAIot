
namespace TemplateAction.Route
{
    public class RegexInlineRouteConstraint : RegexRouteConstraint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegexInlineRouteConstraint" /> class.
        /// </summary>
        /// <param name="regexPattern">The regular expression pattern to match.</param>
        public RegexInlineRouteConstraint(string regexPattern)
            : base(regexPattern)
        {
        }
    }
}
