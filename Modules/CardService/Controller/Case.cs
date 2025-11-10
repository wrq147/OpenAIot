using AuthService.Controller;
using CardService.Business;
using CardService.Model;
using Common;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.NetCore;
using TemplateAction.Route;

namespace CardService.Controller
{
    /// <summary>
    /// 案例API
    /// </summary>
    public class Case : AbstractLoginedController
    {
        private CardCaseBLL _case;
        public Case(CardCaseBLL caseBLL)
        {
            _case = caseBLL;
        }
        /// <summary>
        /// 名片案例列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [TANetValid]
        [HttpGet]
        public async Task<AjaxResult> List(In_Card_Case query)
        {
            return this.Success(await _case.SelectList(query));
        }
        /// <summary>
        /// 获取案例信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> Info(long id)
        {
            return (await _case.SelectById(id)).ToAjaxResult();
        }
        /// <summary>
        /// 管理案例列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [TANetValid]
        [HttpGet]
        public async Task<AjaxResult> ListM(In_Card_Case_M query)
        {
            return this.Success(await _case.SelectListM(query));
        }
        /// <summary>
        /// 管理添加案例
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [TANetValid]
        [HttpPost]
        public async Task<AjaxResult> Add(MZ_Card_Case data)
        {
            return (await _case.Insert(data)).ToAjaxResult();
        }
        /// <summary>
        /// 管理编辑案例
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<AjaxResult> Edit(MZ_Card_Case data)
        {
            return (await _case.Update(data)).ToAjaxResult();
        }
        /// <summary>
        /// 管理删除案例
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> Remove(long id)
        {
            return (await _case.Remove(id)).ToAjaxResult();
        }
    }
}
