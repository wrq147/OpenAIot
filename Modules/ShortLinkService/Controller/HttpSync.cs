using AuthService;
using Common;
using Common.Share;
using DeveloperService;
using DeveloperService.Model;
using ShortLinkService.Business;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Label;
using TemplateAction.Route;

namespace ShortLinkService.Controller
{
    public class HttpSync : AbstractDeveloperController
    {
        private MZ_Developer _develper;
        /// <summary>
        /// 校验开发者权限
        /// </summary>
        /// <param name="ac"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public override async Task<IResult> CallAction(TAAction ac, object[] parameters)
        {
            _develper = GetDeveloper();
            if (_develper.UserType != 1)
            {
                return this.Error<string>(11, "必需为企业开发者");
            }
            return await base.CallAction(ac, parameters);
        }
        /// <summary>
        /// url转短码
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<string>> ToShort(string url)
        {
            var user = _develper.ToUserInfo();
            var shortLinkBLL = this.ServiceProvider.GetService<ShortLinkBLL>();
            return (await shortLinkBLL.Add(url, user.OrgId)).ToAjaxResult();
        }
    }
}
