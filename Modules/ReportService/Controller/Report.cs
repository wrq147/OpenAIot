using AuthService.Controller;
using Common;
using Common.Share;
using Microsoft.Extensions.Logging;
using ReportService.Business;
using ReportService.Models;
using System;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.NetCore;
using TemplateAction.Route;

namespace ReportService.Controller
{
    /// <summary>
    /// 报表设计器接口
    /// </summary>
    public class Report : AbstractLoginedController
    {
        private ILogger<Report> _log;
        private ReportBLL _report;
        public Report(ILoggerFactory factory, ReportBLL report)
        {
            _log = factory.CreateLogger<Report>();
            _report = report;
        }
        /// <summary>
        /// 查询报表列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        [About]
        [TANetWebApiResponse(typeof(DefaultAjaxResult<PageObject<MZ_Report>>))]
        public async Task<AjaxResult> List(In_ReportListPage query)
        {
            return this.Success(await _report.ListPage(query));
        }
        /// <summary>
        /// 获取报表详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [TANetWebApiResponse(typeof(DefaultAjaxResult<MZ_Report>))]
        public async Task<AjaxResult> Info(string id)
        {
            return (await _report.Info(id, GetUser())).ToAjaxResult();
        }

        /// <summary>
        /// 新增报表
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [About]
        [TANetValid]
        public async Task<AjaxResult> Add(MZ_Report data)
        {
            return (await _report.AddReport(data)).ToAjaxResult();
        }
        /// <summary>
        /// 复制指定报表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [About("/ReportService/Report/Add")]
        public async Task<AjaxResult> Copy(long id)
        {
            return (await _report.CopyReport(id)).ToAjaxResult();
        }

        /// <summary>
        /// 修改报表
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [About]
        public async Task<AjaxResult> Edit(MZ_Report data)
        {
            return (await _report.UpdateReport(data)).ToAjaxResult();
        }
        /// <summary>
        /// 删除报表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [About]
        public async Task<AjaxResult> Remove(string id)
        {
            return (await _report.Remove(id)).ToAjaxResult();
        }


    }
}
