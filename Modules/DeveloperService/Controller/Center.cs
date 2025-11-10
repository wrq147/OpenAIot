using AuthService;
using AuthService.Controller;
using Common.Share;
using DeveloperService.Business;
using DeveloperService.Model;
using System;
using System.Threading.Tasks;
using TemplateAction.Route;

namespace DeveloperService.Controller
{
    public class Center : AbstractLoginedController
    {
        private DeveloperBLL _developerBLL;
        public Center(DeveloperBLL developerBLL)
        {
            _developerBLL = developerBLL;
        }
        /// <summary>
        /// 获取当前用户的开发者信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<MZ_Developer>> Profile()
        {
            return (await _developerBLL.Profile()).ToAjaxResult();
        }
    }
}
