using AuthService.Controller;
using Common;
using Common.Share;
using MESService.Business;
using MESService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Route;

namespace MESService.Controller
{
    public class Report : AbstractLoginedController
    {
        private ReportBLL _reportBLL;
        public Report(ReportBLL reportBLL)
        {
            _reportBLL = reportBLL;
        }
        /// <summary>
        /// 报工列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [About]
        [HttpGet]
        [HttpPost]
        public async Task<DefaultAjaxResult<PageObject<MZ_WorkReport>>> List(In_ReportList query)
        {
            return this.Success(await _reportBLL.SelectByPage(query, GetUser()));
        }
        /// <summary>
        /// 生成生产报工编号
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [About("/MESService/Report/List")]
        public async Task<DefaultAjaxResult<string>> GeneratePlaneNumber()
        {
            return this.Success(await _reportBLL.GenerateNumber());
        }

        /// <summary>
        /// 添加生产报工
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [About("/MESService/Report/List")]
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> Add(MZ_WorkReport data)
        {
            return (await _reportBLL.Insert(data, GetUser(), this.IntentAction)).ToAjaxResult();
        }

        /// <summary>
        /// 修改生产报工
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [About("/MESService/Report/List")]
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> Edit(MZ_WorkReport data)
        {
            return (await _reportBLL.Update(data, GetUser(), this.IntentAction)).ToAjaxResult();
        }

        /// <summary>
        /// 删除生产报工（待提交和已取消状态使用）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [About("/MESService/Report/List")]
        [HttpGet]
        public async Task<DefaultAjaxResult<int>> Remove(string id)
        {
            return (await _reportBLL.Delete(id, GetUser())).ToAjaxResult();
        }
        /// <summary>
        /// 提交生产报工
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [About("/MESService/Report/List")]
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> SubmitModel(In_SubmitReport data)
        {
            return (await _reportBLL.SubmitModel(data, GetUser())).ToAjaxResult();
        }
        /// <summary>
        /// 取消生产报工（待审核状态使用）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [About("/MESService/Report/List")]
        public async Task<DefaultAjaxResult<string>> Cancel(string id)
        {
            return (await _reportBLL.Cancel(id, GetUser())).ToAjaxResult();
        }
        /// <summary>
        /// 生成报工的审核表单初始化
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [About("/MESService/Report/List")]
        public async Task<DefaultAjaxResult<Dictionary<string, object>>> FormData(MZ_WorkReport data)
        {
            return (await _reportBLL.CreateFlowForm(data, GetUser())).ToAjaxResult();
        }
    }
}
