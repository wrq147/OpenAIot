using AuthService.Controller;
using Common.Share;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Route;
using WeiXinService.Model;

namespace WeiXinService.Controller
{
    public class Account : AbstractLoginedController
    {
        private WxApiHelper _appletApi;
        public Account(WxApiHelper appletApi)
        {
            _appletApi = appletApi;
        }
        /// <summary>
        /// 获取微信应用列表
        /// </summary>
        /// <returns></returns>
        [About]
        [HttpGet]
        public async Task<DefaultAjaxResult<List<MZ_WXAccount>>> List()
        {
            return this.Success(await _appletApi.AccountList());
        }
        /// <summary>
        /// 添加微信应用
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [About("/WeiXinService/Account/List")]
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> Add(MZ_WXAccount data)
        {
            return (await _appletApi.AddAccount(data)).ToAjaxResult();
        }

        /// <summary>
        /// 删除微信应用
        /// </summary>
        /// <param name="appid"></param>
        /// <returns></returns>
        [About("/WeiXinService/Account/List")]
        [HttpGet]
        public async Task<DefaultAjaxResult<string>> Remove(string appid)
        {
            await _appletApi.RemoveAccount(appid);
            return this.Success<string>();
        }
    }
}
