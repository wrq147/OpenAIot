using Common.Share;
using System;
using TemplateAction.Core;
using TemplateAction.Label;
using System.Threading.Tasks;
using DeveloperService.Model;
using Common;
using DeveloperService.DAL;
using AuthService;

namespace DeveloperService
{
    public class DeveloperMiddleware : IFilterMiddleware
    {
        public DeveloperMiddleware()
        {
        }
        public async Task<IResult> Excute(TAAction ac, FilterMiddlewareNode next)
        {
            bool canAuth = false;
            if (typeof(IDeveloperController).IsAssignableFrom(ac.ControllerNode.ControllerType))
            {
                canAuth = true;
            }
            if (canAuth)
            {
                string tkey = ac.Context.Request.Header["token"];
                if (string.IsNullOrEmpty(tkey))
                {
                    object outstr;
                    if (ac.Context.Request.Query.TryGet("token", out outstr))
                    {
                        tkey = outstr as string;
                    }
                }

                if(string.IsNullOrEmpty(tkey))
                {
                    var userInfo = Data_ServerTokenInfo.From(ac.Context);
                    if (userInfo != null)
                    {
                        tkey = userInfo.DeveloperSecKey;
                    }
                }

                if (string.IsNullOrEmpty(tkey))
                {
                    return new DefaultAjaxResult<string>(80001, "token参数不能为空");
                }

                var localCache = ac.Context.Application.ServiceProvider.GetService<CacheHelper>();
                MZ_Developer tk = localCache.GetCache<MZ_Developer>("developer:" + tkey);
                if (tk == null)
                {
                    var devlist = await ac.Context.Application.ServiceProvider.GetService<DeveloperDAL>().SelectList(x => x.SecKey == tkey);
                    if (devlist.Count == 0)
                    {
                        return new DefaultAjaxResult<string>(80003, "Key验证失败");
                    }
                    tk = devlist[0];
                    localCache.SetCache("developer:" + tkey, tk, DateTime.Now.AddMinutes(30));
                }

                ac.Context.Items["ApiDeveloper"] = tk;
                if (tk.KeyType != 0)
                {
                    return new DefaultAjaxResult<string>(80006, "不支持验证类型");
                }

                return await next.Excute(ac);
            }
            else
            {
                //不需要认证
                return await next.Excute(ac);
            }
        }
    }
}
