using Common;
using Common.Share;
using IoTService.Third.Api;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.NetCore;
using TemplateAction.Route;

namespace IoTService.Controller
{
    /// <summary>
    /// 联通卡回调api
    /// </summary>
    public class UnicomCallback : TANetController
    {
        [HttpGet]
        public async Task<AjaxResult> StausChange()
        {
            return new CallbackResult();
        }
    }
}
