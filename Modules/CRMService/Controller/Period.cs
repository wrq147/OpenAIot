using AuthService.Controller;
using Common.Share;
using CRMService.Business;
using CRMService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;
using Common;
using TemplateAction.Route;
using TemplateAction.NetCore;

namespace CRMService.Controller
{
    /// <summary>
    /// 阶段API
    /// </summary>
    public class Period : AbstractLoginedController
    {
        private PeriodBLL _periodBLL;
        public Period(PeriodBLL periodBLL)
        {
            _periodBLL = periodBLL;
        }
        /// <summary>
        /// 阶段列表
        /// </summary>
        /// <returns></returns>
        [About("/CRMService/Opportunity/List")]
        [HttpGet]
        public async Task<DefaultAjaxResult<List<MZ_Period>>> List()
        {
            return this.Success(await _periodBLL.SelectList());
        }

        /// <summary>
        /// 添加阶段
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [About("/CRMMan/")]
        [HttpPost]
        [TANetValid]
        [Des("新增一条销售阶段")]
        public async Task<DefaultAjaxResult<string>> Add(MZ_Period data)
        {
            return (await _periodBLL.Add(data)).ToAjaxResult();
        }
        /// <summary>
        /// 修改阶段
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [About("/CRMMan/")]
        [HttpPost]
        [TANetValid]
        [Des("修改一条销售阶段")]
        public async Task<DefaultAjaxResult<int>> Edit(MZ_Period data)
        {
            return (await _periodBLL.Edit(data)).ToAjaxResult();
        }
        /// <summary>
        /// 删除阶段
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [About("/CRMMan/")]
        [Des("删除一条销售阶段")]
        public async Task<AjaxResult> Remove(string id)
        {
            return (await _periodBLL.Delete(id)).ToAjaxResult();
        }
    }
}
