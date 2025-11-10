using AuthService.Controller;
using Common;
using Common.Share;
using ReportService.Business;
using ReportService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.NetCore;
using TemplateAction.Route;

namespace ReportService.Controller
{
    /// <summary>
    /// 报表分享接口
    /// </summary>
    public class Share : AbstractLoginedController
    {
        private ShareBLL _shareBLL;
        public Share(ShareBLL shareBLL)
        {
            _shareBLL = shareBLL;
        }
        /// <summary>
        /// 获取分享列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<PageObject<MZ_ReportShare>>> List(In_ShareListPage query)
        {
            return this.Success(await _shareBLL.ListPage(query));
        }
        /// <summary>
        /// 获取分享信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [TANetWebApiResponse(typeof(DefaultAjaxResult<MZ_ReportShare>))]
        public async Task<AjaxResult> Info(string id)
        {
            return (await _shareBLL.Info(id)).ToAjaxResult();
        }
        /// <summary>
        /// 新增分享链接
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [About("/ReportService/Report/List")]
        [TANetValid]
        public async Task<AjaxResult> Add(MZ_ReportShare data)
        {
            return (await _shareBLL.Share(data)).ToAjaxResult();
        }
        /// <summary>
        /// 编辑分享链接
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [About("/ReportService/Report/List")]
        public async Task<AjaxResult> Edit(MZ_ReportShare data)
        {
            return (await _shareBLL.Edit(data)).ToAjaxResult();
        }
        /// <summary>
        /// 批量删除分享链接
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [About("/ReportService/Report/List")]
        [HttpGet]
        public async Task<AjaxResult> Remove(string[] ids)
        {
            return this.Success(await _shareBLL.Delete(ids));
        }
    }
}
