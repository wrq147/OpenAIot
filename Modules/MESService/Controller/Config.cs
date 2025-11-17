using AuthService.Controller;
using Common.Share;
using MESService.Business;
using MESService.Model;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Route;

namespace MESService.Controller
{
    public class Config : AbstractLoginedController
    {
        private ConfigBLL _configBLL;
        public Config(ConfigBLL configBLL)
        {
            _configBLL = configBLL;
        }
        /// <summary>
        /// 设置生产商MES配置信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [About("/MES/")]
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> Set(MZ_FactoryMes data)
        {
            var user = GetUser();
            data.Id = user.OrgId;
            return (await _configBLL.SetFactoryMes(data)).ToAjaxResult();
        }
        /// <summary>
        /// 获取生产商MES配置信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<MZ_FactoryMes>> Info()
        {
            var user = GetUser();
            return (await _configBLL.Info(user.OrgId)).ToAjaxResult();
        }

    }
}
