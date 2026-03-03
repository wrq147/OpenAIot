using AuthService.Controller;
using Common.Share;
using IoTAIService.AIProject;
using IoTAIService.Models;
using System;
using System.Threading.Tasks;
using TemplateAction.Route;
using TemplateAction.Core;
namespace IoTAIService.Controller
{
    /// <summary>
    /// AI项目API
    /// </summary>
    public class Project : AbstractLoginedController
    {
        /// <summary>
        /// 测试AI项目并画检测
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> DrawBoxs(In_TestProject data)
        {
            AIProjectManager man = this.ServiceProvider.GetService<AIProjectManager>();
            var rs = await man.TestDetect(data.Code, data.Base64Img, data.ParamValues);
            return rs.ToAjaxResult();
        }
    }
}
