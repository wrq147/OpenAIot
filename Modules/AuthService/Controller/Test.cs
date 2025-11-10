using Common;
using Common.Share;
using System;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.NetCore;
using TemplateAction.Route;

namespace AuthService.Controller
{
    public class Test : TANetController
    {
        /// <summary>
        /// 测试网络用
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> IsOk()
        {
            return this.Success("ok");
        }
        /// <summary>
        /// 判断是否启用指定服务
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<bool>> IsServiceStart(string name)
        {
            return this.Success(this.Context.Application.ExistPlugin("CRMService"));
        }
    }
}
