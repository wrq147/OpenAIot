using AuthService.Controller;
using Common.Share;
using CRMService.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Route;
using Common;
using CRMService.Model;

namespace CRMService.Controller
{
    /// <summary>
    /// CRM报表接口
    /// </summary>
    public class CrmReport : AbstractLoginedController
    {
        private CrmReportBLL _reportBLL;
        public CrmReport(CrmReportBLL reportBLL)
        {
            _reportBLL = reportBLL;
        }

        /// <summary>
        /// 获取CRM统计信息
        /// </summary>
        /// <param name="day">0为今天、1为昨天、7为近7天、30为近30天</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<Out_CrmStatistics>> StatisticsInfo(int day)
        {
            return this.Success(await _reportBLL.StatisticsInfo(day));
        }
        /// <summary>
        /// 获取CRM总计信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<Out_CrmCount>> StatisticsCount()
        {
            return (await _reportBLL.StatisticsCount()).ToAjaxResult();
        }
        /// <summary>
        /// 获取销售漏斗信息
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<List<Out_PeriodCount>>> PeriodCountList(int day)
        {
            return this.Success(await _reportBLL.PeriodCountList(day));
        }
     
    }
}
