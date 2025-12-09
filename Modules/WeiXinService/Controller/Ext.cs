using AuthService;
using Common.Share;
using System;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.NetCore;
using TemplateAction.Route;
using WeiXinService.Model;

namespace WeiXinService.Controller
{
    /// <summary>
    /// 微信扩展操作
    /// </summary>
    public class Ext : TANetController
    {
        public Ext()
        {
        }

        /// <summary>
        /// 获取微信小程序的跳转加密URL Scheme
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<string>> WxSchemeTick(In_GenerateWxSchemeTick data)
        {
            WxApiHelper apiHelper = this.ServiceProvider.GetService<WxApiHelper>();
            string appid = await apiHelper.DefaultAppId(data.appid);
            var rs = await apiHelper.GenerateWxSchemeTick(appid, data.path, data.query);
            return rs.ToAjaxResult();
        }
    }
}
