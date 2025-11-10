using CardService.Business;
using CardService.Model;
using DeveloperService;
using System;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.NetCore;
using TemplateAction.Route;

namespace CardService.Controller
{
    /// <summary>
    /// 供第三方调用Api接口
    /// </summary>
    public class Api : AbstractDeveloperController
    {
        private CardOrgBLL _cardOrgBLL;
        public Api(CardOrgBLL cardOrgBLL)
        {
            _cardOrgBLL = cardOrgBLL;
        }
        /// <summary>
        /// 判断是否有创建第三方企业Id
        /// </summary>
        /// <param name="dev">开发者Id</param>
        /// <param name="id">第三方企业Id</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> JudgeBindId(string dev, string id)
        {
            return (await _cardOrgBLL.JudgeBindId(dev, id)).ToAjaxResult();
        }
    }
}
