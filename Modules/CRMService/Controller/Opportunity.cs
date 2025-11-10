using AuthService.Controller;
using Common.Share;
using Common;
using CRMService.Business;
using CRMService.Model;
using System;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Route;
using TemplateAction.NetCore;

namespace CRMService.Controller
{
    /// <summary>
    /// 商机API接口
    /// </summary>
    public class Opportunity : AbstractLoginedController
    {
        private OpportunityBLL _opportBLL;
        public Opportunity(OpportunityBLL opportBLL)
        {
            _opportBLL = opportBLL;
        }
        /// <summary>
        /// 商机列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [About]
        [HttpGet]
        public async Task<DefaultAjaxResult<PageObject<MZ_Opportunity>>> List(In_OpportunityList query)
        {
            return this.Success(await _opportBLL.SelectList(query));
        }
        /// <summary>
        /// 获取商机
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<MZ_Opportunity>> Info(string id)
        {
            return (await _opportBLL.Info(id)).ToAjaxResult();
        }

        /// <summary>
        /// 添加商机
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [About]
        [HttpPost]
        [TANetValid]
        public async Task<DefaultAjaxResult<string>> Add(MZ_Opportunity data)
        {
            return (await _opportBLL.Add(data)).ToAjaxResult();
        }
        /// <summary>
        /// 修改商机
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [About]
        [HttpPost]
        [TANetValid]
        public async Task<DefaultAjaxResult<string>> Edit(MZ_Opportunity data)
        {
            return (await _opportBLL.Edit(data)).ToAjaxResult();
        }
        /// <summary>
        /// 推进商机
        /// </summary>
        /// <param name="id"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        [About("/CRMService/Opportunity/Edit")]
        [HttpGet]
        public async Task<DefaultAjaxResult<string>> Forward(string id, string target)
        {
            return (await _opportBLL.Forward(id, target)).ToAjaxResult();
        }
        /// <summary>
        /// 生成商机编号
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [About("/CRMService/Opportunity/List")]
        public async Task<DefaultAjaxResult<string>> GenerateNumber()
        {
            return this.Success(await _opportBLL.GenerateNumber());
        }
        /// <summary>
        /// 删除商机
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [About("/CRMService/Opportunity/Edit")]
        [HttpGet]
        public async Task<DefaultAjaxResult<string>> Remove(string id)
        {
            return (await _opportBLL.Delete(id)).ToAjaxResult();
        }
    }
}
