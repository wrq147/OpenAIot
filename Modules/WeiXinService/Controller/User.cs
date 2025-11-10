using AuthService.Controller;
using System;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Route;
using WeiXinService.Business;
using WeiXinService.Model;

namespace WeiXinService.Controller
{
    /// <summary>
    /// 微信用户相关API
    /// </summary>
    public class User : AbstractLoginedController
    {
        private WeiXinBLL _wxBLL;
        public User(WeiXinBLL wxBLL)
        {
            _wxBLL = wxBLL;
        }


        /// <summary>
        /// 绑定手机号
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> MobileBind(In_MobileBind data)
        {
            return (await _wxBLL.MobileBind(data.appid, data.code)).ToAjaxResult();
        }
        /// <summary>
        /// 绑定微信小程序
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> WxAppletBind(In_WxAppletBind data)
        {
            return (await _wxBLL.WxAppletBind(data.appid, data.code, GetUser())).ToAjaxResult();
        }
        /// <summary>
        /// 解绑微信小程序
        /// </summary>
        /// <param name="appid"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> WxAppletUnBind(string appid)
        {
            return (await _wxBLL.WxAppletUnBind(appid, GetUser())).ToAjaxResult();
        }
    }
}
