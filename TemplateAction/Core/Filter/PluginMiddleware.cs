using System;
using System.Threading.Tasks;
using TemplateAction.Label;

namespace TemplateAction.Core
{
    /// <summary>
    /// 为插件注册的中间件提供热更新
    /// </summary>
    internal class PluginMiddleware : IFilterMiddleware
    {
        private string _key;
        public PluginMiddleware(string key)
        {
            _key = key;
        }
        public async Task<IResult> Excute(TAAction ac, FilterMiddlewareNode next)
        {
            IFilterMiddleware filter = ac.Context.Application.ServiceProvider.GetService(_key, ac) as IFilterMiddleware;
            if (filter != null)
            {
                return await filter.Excute(ac, next);
            }
            else
            {
                return await next.Excute(ac);
            }
        }
    }
}
