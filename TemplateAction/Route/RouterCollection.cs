
using System;
using System.Collections.Generic;
using TemplateAction.Core;

namespace TemplateAction.Route
{
    public class RouterCollection : IRouterCollection<IRouter>
    {
        /// <summary>
        /// 插件路由集合
        /// </summary>
        private List<IRouter> _routers;
        public RouterCollection()
        {
            _routers = new List<IRouter>();
        }
        public void Add(IRouter router)
        {
            _routers.Add(router);
        }

        public virtual IDictionary<string, object> Route(ITAContext context)
        {
            foreach (IRouter router in _routers)
            {
                IDictionary<string, object> rt = router.Route(context);
                if (rt != null)
                {
                    return rt;
                }
            }
            return null;
        }
    }
}
