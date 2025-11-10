using System;
using System.Collections.Generic;


namespace TemplateAction.Core
{
    public class PluginRouterCollection : IRouterCollection<IPluginRouter>
    {
        private List<IPluginRouter> _routers;
        public PluginRouterCollection()
        {
            _routers = new List<IPluginRouter>();
        }

        public void Add(IPluginRouter router)
        {
            int insertPos = 0;
            for(int i = _routers.Count - 1; i >= 0; i--)
            {
                IPluginRouter r = _routers[i];
                if (router.SortIndex >= r.SortIndex)
                {
                    insertPos = i + 1;
                    break;
                }
            }
            _routers.Insert(insertPos, router);
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
