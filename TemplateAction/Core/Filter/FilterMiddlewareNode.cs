using System;
using System.Threading.Tasks;
using TemplateAction.Label;

namespace TemplateAction.Core
{
    public class FilterMiddlewareNode
    {
        private FilterMiddlewareNode _next;
        public FilterMiddlewareNode Next
        {
            get { return _next; }
            internal set { _next = value; }
        }
        private IFilterMiddleware _filter;
        public FilterMiddlewareNode(IFilterMiddleware filter)
        {
            _filter = filter;
        }

        public virtual async Task<IResult> Excute(TAAction ac)
        {
            return await _filter.Excute(ac, _next);
        }
    }
}
