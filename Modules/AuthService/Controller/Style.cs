using AuthService.Model;
using Common.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.NetCore;
using TemplateAction.Route;

namespace AuthService.Controller
{
    public class Style : TANetController
    {
        private StyleBLL _styleBLL;
        public Style(StyleBLL styleBLL)
        {
            _styleBLL = styleBLL;
        }
        /// <summary>
        /// 根据企业Id获取当前企业的主题
        /// </summary>
        /// <param name="orgId">企业Id</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<MZ_AppStyle>> OrgStyle(long orgId)
        {
            return (await _styleBLL.OrgStyle(orgId)).ToAjaxResult();
        }
    }
}
