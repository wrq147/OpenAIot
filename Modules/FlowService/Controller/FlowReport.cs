using AuthService.Controller;
using Common.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Route;
using Common;
using FlowService.Business;
using FlowService.Model;

namespace FlowService.Controller
{
    /// <summary>
    /// 流程报表接口
    /// </summary>
    public class FlowReport : AbstractLoginedController
    {
        private FlowReportBLL _reportBLL;
        public FlowReport(FlowReportBLL reportBLL)
        {
            _reportBLL = reportBLL;
        }
        /// <summary>
        /// 首页工作台统计信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<Out_FlowStatistics>> StatisticsInfo()
        {
            return this.Success(await _reportBLL.StatisticsInfo());
        }
    }
}
