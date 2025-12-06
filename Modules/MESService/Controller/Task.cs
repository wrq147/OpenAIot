using AuthService.Controller;
using Common;
using Common.Share;
using MESService.Business;
using MESService.Model;
using System;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Route;

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
        /// <summary>
        /// 获取生产任务信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<MZ_WorkTask>> Info(string id)
        {
            return (await _workTaskBLL.Info(id)).ToAjaxResult();
        }
    }
}
