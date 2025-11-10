using AuthService.Model;
using Common;
using Common.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.NetCore;
using TemplateAction.Route;

namespace AuthService.Controller
{
    /// <summary>
    /// 应用主题管理
    /// </summary>
    public class StyleMan : AbstractLoginedController
    {
        private StyleBLL _styleBLL;
        public StyleMan(StyleBLL styleBLL)
        {
            _styleBLL = styleBLL;
        }
        /// <summary>
        /// 应用主题列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [About]
        [HttpGet]
        public async Task<DefaultAjaxResult<PageObject<MZ_AppStyle>>> List(In_StyleList query)
        {
            return this.Success(await _styleBLL.SelectList(query));
        }

        /// <summary>
        /// 主题的企业列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [About("/AuthService/StyleMan/List")]
        [HttpGet]
        public async Task<DefaultAjaxResult<PageObject<MZ_Org>>> StyleOrgList(In_StyleOrgList query)
        {
            return this.Success(await _styleBLL.StyleOrgList(query));
        }
        /// <summary>
        /// 添加应用主题
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [About("/AuthService/StyleMan/List")]
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> Add(MZ_AppStyle data)
        {
            return (await _styleBLL.Add(data)).ToAjaxResult();
        }

        /// <summary>
        /// 修改应用主题
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [About("/AuthService/StyleMan/List")]
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> Edit(MZ_AppStyle data)
        {
            return (await _styleBLL.Edit(data)).ToAjaxResult();
        }

        /// <summary>
        /// 删除应用主题
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [About("/AuthService/StyleMan/List")]
        [HttpGet]
        public async Task<DefaultAjaxResult<string>> Remove(string id)
        {
            return (await _styleBLL.Delete(id)).ToAjaxResult();
        }
        /// <summary>
        /// 获取应用主题
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<MZ_AppStyle>> Info(string id)
        {
            return (await _styleBLL.Info(id)).ToAjaxResult();
        }

        /// <summary>
        /// 给企业分配指定主题
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="styleId"></param>
        /// <returns></returns>
        [About("/AuthService/StyleMan/List")]
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> SetOrgStyle(long orgId, string styleId)
        {
            MZ_OrgStyle style = new MZ_OrgStyle();
            style.OrgId = orgId;
            style.StyleId = styleId;
            return (await _styleBLL.SetOrgStyle(style)).ToAjaxResult();
        }
        /// <summary>
        /// 删除企业的指定主题
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="styleId"></param>
        /// <returns></returns>
        [About("/AuthService/StyleMan/List")]
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> DelOrgStyle(long orgId, string styleId)
        {
            return (await _styleBLL.DelOrgStyle(orgId, styleId)).ToAjaxResult();
        }
    }
}
