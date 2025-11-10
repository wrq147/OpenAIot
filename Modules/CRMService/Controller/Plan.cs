using AuthService.Controller;
using Common.Share;
using Common;
using CRMService.Business;
using CRMService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Route;
using TemplateAction.NetCore;

namespace CRMService.Controller
{
    /// <summary>
    /// 跟进计划API
    /// </summary>
    public class Plan : AbstractLoginedController
    {
        private PlanBLL _planBLL;
        public Plan(PlanBLL plan)
        {
            _planBLL = plan;
        }
        /// <summary>
        /// 跟进计划列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [About]
        [HttpGet]
        public async Task<DefaultAjaxResult<PageObject<MZ_FollowPlan>>> List(In_FollowPlanList query)
        {
            return this.Success(await _planBLL.SelectList(query));
        }

        /// <summary>
        /// 获取跟进计划
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<MZ_FollowPlan>> Info(string id)
        {
            return (await _planBLL.Info(id)).ToAjaxResult();
        }
        /// <summary>
        /// 添加跟进计划
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [About]
        [HttpPost]
        [TANetValid]
        public async Task<DefaultAjaxResult<string>> Add(MZ_FollowPlan data)
        {
            return (await _planBLL.Add(data)).ToAjaxResult();
        }
        /// <summary>
        /// 修改跟进计划
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [About]
        [HttpPost]
        [TANetValid]
        public async Task<DefaultAjaxResult<string>> Edit(MZ_FollowPlan data)
        {
            return (await _planBLL.Edit(data)).ToAjaxResult();
        }

        /// <summary>
        /// 删除跟进计划
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [About]
        [HttpGet]
        public async Task<DefaultAjaxResult<string>> Remove(string id)
        {
            return (await _planBLL.Delete(id)).ToAjaxResult();
        }
        /// <summary>
        /// 完成跟进计划
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [About]
        [HttpGet]
        public async Task<DefaultAjaxResult<string>> Finish(string id)
        {
            return (await _planBLL.Finish(id)).ToAjaxResult();
        }
    }
}
