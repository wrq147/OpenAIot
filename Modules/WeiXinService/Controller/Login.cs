using TemplateAction.Core;
using TemplateAction.NetCore;
using TemplateAction.Route;
using System.Threading.Tasks;
using WeiXinService.Model;
using WeiXinService.Business;
using Common.Share;
using AuthService;
using log4net;
using System;
using Common;

namespace WeiXinService.Controller
{
    /// <summary>
    /// 微信登录API
    /// </summary>
    public class Login : TANetController
    {
        private WeiXinBLL _wxBLL;
        public Login(WeiXinBLL wxBLL)
        {
            _wxBLL = wxBLL;
        }

        private string FilterUrlParmcode(string strCond)
        {
            string RedirectUri = strCond.Substring(0, strCond.IndexOf('?'));
            string parmuri = strCond.Substring(strCond.IndexOf('?') + 1);
            string parms = "";
            if (parmuri.Length > 0)
            {
                string[] uriobjs = parmuri.Split('&');
                for (int i = 0; i < uriobjs.Length; i++)
                {
                    if (uriobjs[i].IndexOf("code") == -1)
                    {
                        parms = parms + uriobjs[i] + "&";
                    }
                }
                if (parms.Length > 0)
                {
                    parms = parms.Substring(0, parms.Length - 1);
                    RedirectUri = RedirectUri + "?" + parms;
                }
            }
            return RedirectUri;
        }

        /// <summary>
        /// 微信小程序登录
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<AjaxResult> FromWxApplet(In_LoginApplet data)
        {
            BusResponse<Out_Login> response = await _wxBLL.AppletLogin(data, Context);
            return response.ToAjaxResult();
        }


        /// <summary>
        /// 构造企业微信网页授权链接
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> CreateWxCorpRedirectUrl(string appId, string url)
        {
            string state = Guid.NewGuid().ToString().Replace("-", "");
            string RedirectUri = url;
            if (url.IndexOf('?') > 0)//过滤掉网址上的code参数
            {
                RedirectUri = FilterUrlParmcode(url);
            }

            string send_url = "https://open.weixin.qq.com/connect/oauth2/authorize?appid=" + appId +
                "&redirect_uri=" + System.Web.HttpUtility.UrlEncode(RedirectUri) + "&response_type=code&scope=snsapi_base&state=" + state + "#wechat_redirect";

            return this.Success<string>(send_url);
        }
        /// <summary>
        /// 企业微信登录
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<AjaxResult> FromWxCorp(In_LoginCorp data)
        {
            BusResponse<Out_Login> response = await _wxBLL.CorpLogin(data, Context);
            return response.ToAjaxResult();
        }

        /// <summary>
        /// 获取微信JSSDK配置信息
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<AjaxResult> WxConfigJson(string appid, string url)
        {
            var api = this.ServiceProvider.GetService<WxApiHelper>();
            return (await api.GetWxConfigJson(appid, url)).ToAjaxResult();
        }


        /// <summary>
        /// 获取企业微信JSSDK配置信息
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<AjaxResult> CorpWxConfigJson(string appid, string url)
        {
            var api = this.ServiceProvider.GetService<WxApiHelper>();
            return (await api.GetCorpWxConfigJson(appid, url)).ToAjaxResult();
        }
    }
}
