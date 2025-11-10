using AuthService.Controller;
using Common.Share;
using IoTService.Business;
using IoTService.Models;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.NetCore;
using TemplateAction.Route;
using Common;
namespace IoTService.Controller
{
    /// <summary>
    /// 物联第三方接入配置
    /// </summary>
    public class IotConfig : AbstractLoginedController
    {
        private IotConfigBLL _configBLL;
        public IotConfig(IotConfigBLL configBLL)
        {
            _configBLL = configBLL;
        }
        /// <summary>
        /// 修改物联第三方接入配置
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [About()]
        public async Task<DefaultAjaxResult<int>> Update(In_IotConfig data)
        {
            var user = GetUser();
            MZ_IotConfig config = new MZ_IotConfig();
            config.EnableAutoAdd = data.EnableAutoAdd;
            config.SimBossOption = data.SimBossOption.Trim();
            config.YiDongOption = data.YiDongOption.Trim();
            config.SohanOption = data.SohanOption.Trim();
            config.UnicomOption = data.UnicomOption.Trim();
            config.OrgId = user.OrgId;
            return (await _configBLL.UpdateConfig(config)).ToAjaxResult();
        }
        /// <summary>
        /// 获取当前的物联配置信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<MZ_IotConfig>> Info()
        {
            var user = GetUser();
            return this.Success(await _configBLL.Info(user.OrgId));
        }
    }
}
