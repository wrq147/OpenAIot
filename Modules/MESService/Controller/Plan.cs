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
    /// 生产计划API
    /// </summary>
    public class Plan : AbstractLoginedController
    {
        private PlanBLL _planBLL;
        public Plan(PlanBLL planBLL)
        {
            _planBLL = planBLL;
        }
        /// <summary>
        /// 生产计划列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [About]
        [HttpGet]
        public async Task<DefaultAjaxResult<PageObject<MZ_ProductPlan>>> List(In_PlanList query)
        {
            return this.Success(await _planBLL.SelectList(query, GetUser()));
        }

        /// <summary>
        /// 获取生产计划信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<MZ_ProductPlan>> Info(string id)
        {
            return (await _planBLL.Info(id)).ToAjaxResult();
        }
        /// <summary>
        /// 添加生产计划
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [About("/MESService/Plan/List")]
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> Add(MZ_ProductPlan data)
        {
            return (await _planBLL.Insert(data, GetUser())).ToAjaxResult();
        }

        /// <summary>
        /// 修改生产计划
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [About("/MESService/Plan/List")]
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> Edit(MZ_ProductPlan data)
        {
            return (await _planBLL.Update(data, GetUser())).ToAjaxResult();
        }

        /// <summary>
        /// 删除生产计划（待提交和已取消状态使用）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [About("/MESService/Plan/List")]
        [HttpGet]
        public async Task<DefaultAjaxResult<int>> Remove(string id)
        {
            return (await _planBLL.Delete(id, GetUser())).ToAjaxResult();
        }
        /// <summary>
        /// 生成生产计划编号
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [About("/MESService/Plan/List")]
        public async Task<DefaultAjaxResult<string>> GeneratePlaneNumber()
        {
            return this.Success(await _planBLL.GenerateNumber());
        }

        /// <summary>
        /// 提交生产计划
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [About("/MESService/Plan/List")]
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> SubmitModel(In_SubmitPlan data)
        {
            return (await _planBLL.SubmitModel(data, GetUser())).ToAjaxResult();
        }
        /// <summary>
        /// 取消生产计划（待审核状态使用）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [About("/MESService/Plan/List")]
        public async Task<DefaultAjaxResult<string>> Cancel(string id)
        {
            return (await _planBLL.Cancel(id, GetUser())).ToAjaxResult();
        }
        /// <summary>
        /// 生产计划的审核表单初始化
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [About("/StorageService/Apply/List")]
        public async Task<DefaultAjaxResult<Dictionary<string, object>>> FormData(MZ_ProductPlan data)
        {
            return (await _planBLL.CreateFlowForm(data, GetUser())).ToAjaxResult();
        }

    }
}
