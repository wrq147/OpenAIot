using AuthService;
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
using TemplateAction.Route;

namespace ReportService.Controller
{
    /// <summary>
    /// 接口数据源Api
    /// </summary>
    public class ApiSource : AbstractLoginedController
    {
        private ApiSourceBLL _sourceBLL;
        public ApiSource(ApiSourceBLL sourceBLL)
        {
            _sourceBLL = sourceBLL;
        }
        /// <summary>
        /// 获取当前企业的接口数据源
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        [About("/ReportService/Source/List")]
        public async Task<DefaultAjaxResult<PageObject<MZ_ApiSource>>> List(In_ApiSourceListPage query)
        {
            return this.Success(await _sourceBLL.ListPage(query));
        }

        /// <summary>
        /// 获取接口数据源
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<MZ_ApiSource>> Info(string id)
        {
            return (await _sourceBLL.Info(id)).ToAjaxResult();
        }

        /// <summary>
        /// 新增接口数据源
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [About("/ReportService/Source/Add")]
        public async Task<AjaxResult> Add(MZ_ApiSource data)
        {
            return (await _sourceBLL.Add(data)).ToAjaxResult();
        }


        /// <summary>
        /// 修改接口数据源
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [About("/ReportService/Source/Edit")]
        public async Task<AjaxResult> Edit(MZ_ApiSource data)
        {
            return (await _sourceBLL.Update(data)).ToAjaxResult();
        }
        /// <summary>
        /// 删除接口数据源
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [About("/ReportService/Source/Remove")]
        public async Task<AjaxResult> Remove(string id)
        {
            return (await _sourceBLL.Remove(id)).ToAjaxResult();
        }
    }
}
