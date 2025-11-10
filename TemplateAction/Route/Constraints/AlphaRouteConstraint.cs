
namespace TemplateAction.Route
{
    public class AlphaRouteConstraint : RegexRouteConstraint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AlphaRouteConstraint" /> class.
        /// </summary>
        public AlphaRouteConstraint() : base(@"^[a-z]*$")
        {
        }
    }
}
