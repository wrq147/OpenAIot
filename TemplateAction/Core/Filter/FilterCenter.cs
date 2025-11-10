using System;
using System.Threading.Tasks;
using TemplateAction.Label;

namespace TemplateAction.Core
{
    public class FilterCenter
    {
        /// <summary>
        /// Action拦截器中间件列表
        /// </summary>
        private FilterMiddlewareNode _first;
        private FilterMiddlewareNode _last;
        private FilterMiddlewareNode _mvc;
        public FilterCenter()
        {
            //添加Mvc中间件
            FilterMiddlewareNode node = new FilterMiddlewareNode(new MvcMiddleware());
            node.Next = null;
            _first = node;
            _mvc = node;
            _last = null;
        }

        public async Task<IResult> Excute(TAAction request)
        {
            return await _first.Excute(request);
        }
        /// <summary>
        /// 添加在最后， 也就是mvc前面
        /// </summary>
        /// <param name="filter"></param>
        public void Add(IFilterMiddleware filter)
        {
            FilterMiddlewareNode node = new FilterMiddlewareNode(filter);
            node.Next = _mvc;
            if (_last == null)
            {
                _last = node;
                _first = node;
            }
            else
            {
                _last.Next = node;
                _last = node;
            }
        }
        /// <summary>
        /// 添加在第一个位置，也就是mvc之前
        /// </summary>
        /// <param name="filter"></param>
        public void AddFirst(IFilterMiddleware filter)
        {
            FilterMiddlewareNode node = new FilterMiddlewareNode(filter);
            node.Next = _first;
            _first = node;
            if (_last == null)
            {
                _last = node;
            }
        }
    }
}
