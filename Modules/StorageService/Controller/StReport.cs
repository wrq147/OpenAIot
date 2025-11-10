using AuthService.Controller;
using Common;
using Common.Share;
using StorageService.Business;
using StorageService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Route;

namespace StorageService.Controller
{
    /// <summary>
    /// 仓储报表接口
    /// </summary>
    public class StReport : AbstractLoginedController
    {
        private StReportBLL _reportBLL;
        public StReport(StReportBLL reportBLL)
        {
            _reportBLL = reportBLL;
        }
        /// <summary>
        /// 库存信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<Out_StockStatistics>> StockInfo()
        {
            return this.Success(await _reportBLL.StockInfo());
        }
        /// <summary>
        /// 库存记录列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<PageObject<Out_StockRecord>>> DetailRecords(In_DetailRecordPage query)
        {
            return this.Success(await _reportBLL.SelectDetailByPage(query));
        }

    }
}
