using AuthService.Controller;
using Common.Share;
using Common;
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
    /// <summary>
    /// 生产工单API
    /// </summary>
    public class Order : AbstractLoginedController
    {
        private WorkOrderBLL _workOrderBLL;
        public Order(WorkOrderBLL workOrderBLL)
        {
            _workOrderBLL = workOrderBLL;
        }
        /// <summary>
        /// 生产工单列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [About]
        [HttpGet]
        public async Task<DefaultAjaxResult<PageObject<MZ_WorkOrder>>> List(In_WorkOrderList query)
        {
            return this.Success(await _workOrderBLL.SelectList(query, GetUser()));
        }
    }
}
