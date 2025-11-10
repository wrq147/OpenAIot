using AuthService;
using Common.Share;using System;
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
        private ConfigBLL _config;
        public Ext(ConfigBLL config)
        {
            _config = config;
        }
        /// <summary>
        /// 短信跳转小程序中转页面
        /// </summary>
        /// <returns></returns>
        public async Task<ViewResult> SmsJmp()
        {
            string jpage = await _config.SelectConfigByKey("smsjmp");
            return new ViewResult(jpage);
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
