using AuthService.Controller;
using Common;
using Common.Share;
using CRMService.Business;
using CRMService.Model;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Route;

namespace CRMService.Controller
{
    /// <summary>
    /// CRM配置API
    /// </summary>
    public class Config : AbstractLoginedController
    {
        private CrmConfBLL _crmConfBLL;
        public Config(CrmConfBLL crmConfBLL)
        {
            _crmConfBLL = crmConfBLL;
        }

        /// <summary>
        /// CRM设置
        /// </summary>
        /// <param name="conf"></param>
        /// <returns></returns>
        [About]
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> SetCrm(MZ_CrmConf conf)
        {
            return (await _crmConfBLL.SetCrm(conf)).ToAjaxResult();
        }

        /// <summary>
        /// 获取CRM设置
        /// </summary>
        /// <param name="id">指定企业Id,为0表示当前企业</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<MZ_CrmConf>> CrmInfo(long id = 0)
        {
            if (id == 0)
            {
                id = GetUser().OrgId;
            }
            return this.Success(await _crmConfBLL.CrmInfo(id));
        }

    }
}
