using AuthService.Controller;
using Common.Share;
using EfficiencyService.Business;
using EfficiencyService.Model.Org;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Route;

namespace EfficiencyService.Controller
{
    public class OrgConf : AbstractLoginedController
    {
        private OrgConfBLL _orgConfBLL;
        public OrgConf(OrgConfBLL orgConfBLL)
        {
            _orgConfBLL = orgConfBLL;
        }

        /// <summary>
        /// 保存能碳企业配置信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> Save(T_OrgConf data)
        {
            return (await _orgConfBLL.SaveOrgConf(data, GetUser())).ToAjaxResult();
        }

        /// <summary>
        /// 获取能碳的企业配置信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<T_OrgConf>> Info()
        {
            return (await _orgConfBLL.GetOrgConf(GetUser())).ToAjaxResult();
        }
    }
}
