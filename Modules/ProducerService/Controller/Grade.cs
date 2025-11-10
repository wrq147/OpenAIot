using ProducerService.Business;
using ProducerService.Model;
using AuthService.Controller;
using Common.Share;
using Common;
using System;
using TemplateAction.Core;
using TemplateAction.Route;
using System.Threading.Tasks;
using System.Collections.Generic;
namespace ProducerService.Controller
{
    /// <summary>
    /// 代理级别API
    /// </summary>
    public class Grade : AbstractLoginedController
    {
        private GradeBLL _gradeBLL;
        public Grade(GradeBLL gradeBLL)
        {
            _gradeBLL = gradeBLL;
        }
        /// <summary>
        /// 级别列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [About]
        public async Task<DefaultAjaxResult<List<MZ_Grade>>> List()
        {
            return this.Success(await _gradeBLL.SelectList());
        }
        /// <summary>
        /// 级别排序
        /// </summary>
        /// <param name="ids">排序Id列表</param>
        /// <returns></returns>
        [HttpPost]
        [About("/AgentMan/")]
        public async Task<DefaultAjaxResult<int>> Sort(List<string> ids)
        {
            return (await _gradeBLL.UpdateSort(ids)).ToAjaxResult();
        }
        /// <summary>
        /// 新增级别
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [About("/AgentMan/")]
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> Add(MZ_Grade data)
        {
            return (await _gradeBLL.Add(data)).ToAjaxResult();
        }

        /// <summary>
        /// 编辑级别
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [About("/AgentMan/")]
        public async Task<DefaultAjaxResult<int>> Edit(MZ_Grade data)
        {
            return (await _gradeBLL.Update(data)).ToAjaxResult();
        }


        /// <summary>
        /// 删除级别
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [About("/AgentMan/")]
        [HttpGet]
        public async Task<DefaultAjaxResult<string>> Remove(string id)
        {
            return (await _gradeBLL.Delete(id)).ToAjaxResult();
        }
    }
}
