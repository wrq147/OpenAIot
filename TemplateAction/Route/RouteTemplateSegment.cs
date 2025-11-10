using System.Collections.Generic;

namespace TemplateAction.Route
{
    public class RouteTemplateSegment
    {
        public bool IsSimple => Parts.Count == 1;

        public List<RouteTemplatePart> Parts { get; } = new List<RouteTemplatePart>();

    }
}
