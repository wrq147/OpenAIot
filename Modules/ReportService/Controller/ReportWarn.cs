using AuthService.Controller;
using Common;
using Common.Share;
using Microsoft.Extensions.Logging;
using ReportService.Business;
using ReportService.Models;
using System;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Route;

namespace ReportService.Controller
{
    public class ReportWarn : AbstractLoginedController
    {
        private ILogger<ReportWarn> _log;
        private ReportWarnBLL _reportWarn;
        public ReportWarn(ILoggerFactory factory, ReportWarnBLL reportWarn)
        {
            _log = factory.CreateLogger<ReportWarn>();
            _reportWarn = reportWarn;
        }
        /// <summary>
        /// 数据告警列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        [About("/ReportService/Report/Add,/ReportService/Report/Edit")]
        public async Task<AjaxResult> List(In_ReportWarnPage query)
        {
            return this.Success(await _reportWarn.ListPage(query));
        }

        /// <summary>
        /// 获取数据告警信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<MZ_ReportWarn>> Info(string id)
        {
            return (await _reportWarn.Info(id)).ToAjaxResult();
        }

        /// <summary>
        /// 新增数据告警
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [About("/ReportService/Report/Add")]
        public async Task<AjaxResult> Add(MZ_ReportWarn data)
        {
            return (await _reportWarn.Add(data, GetUser())).ToAjaxResult();
        }


        /// <summary>
        /// 修改数据告警
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [About("/ReportService/Report/Edit")]
        public async Task<AjaxResult> Edit(MZ_ReportWarn data)
        {
            return (await _reportWarn.Update(data, GetUser())).ToAjaxResult();
        }
        /// <summary>
        /// 删除数据告警
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [About("/ReportService/Report/Remove")]
        public async Task<AjaxResult> Remove(string id)
        {
            return (await _reportWarn.Remove(id, GetUser())).ToAjaxResult();
        }

    }
}
