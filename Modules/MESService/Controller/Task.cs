using AuthService.Controller;
using Common.Share;
using Common;
using MESService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Route;
using MESService.Business;

namespace MESService.Controller
{
    /// <summary>
    /// 生产任务API
    /// </summary>
    public class Task : AbstractLoginedController
    {
        private WorkTaskBLL _workTaskBLL;
        public Task(WorkTaskBLL workTaskBLL)
        {
            _workTaskBLL = workTaskBLL;
        }
        /// <summary>
        /// 生产任务列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [About]
        [HttpGet]
        public async Task<DefaultAjaxResult<PageObject<MZ_WorkTask>>> List(In_WorkTaskList query)
        {
            return this.Success(await _workTaskBLL.SelectList(query, GetUser()));
        }
    }
}
