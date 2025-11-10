using Common.Share;
using TemplateAction.Core;
using TemplateAction.NetCore;
using TemplateAction.Route;
using System.Threading.Tasks;
using AuthService.Model;
using System.Collections.Generic;

namespace AuthService.Controller
{

    public class SysLogin : TANetController
    {
        private AuthBLL _authBLL;
        private OrgBLL _orgBLL;

        public SysLogin(AuthBLL auth, OrgBLL orgBLL)
        {
            _authBLL = auth;
            _orgBLL = orgBLL;
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<AjaxResult> Login(In_Login ipt)
        {
            BusResponse<Out_Login> response = await _authBLL.Login(ipt, Context);
            return response.ToAjaxResult();
        }

        /// <summary>
        /// 登出后，可直接使用刷新令牌重新登录
        /// </summary>
        /// <param name="refreshtk"></param>
        /// <param name="clientToken"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> RefreshToken(string refreshtk, string clientToken)
        {
            return (await _authBLL.RefreshToken(refreshtk, clientToken, Context)).ToAjaxResult();
        }
        /// <summary>
        /// 获取图形验证码
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public AjaxResult CaptchaImage()
        {
            return _authBLL.GenerateCaptchaImage().ToAjaxResult();
        }


        /// <summary>
        /// 远程验证令牌权限
        /// </summary>
        /// <param name="permis">权限点</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> Check(string permis = "")
        {
            BusResponse<Data_ServerTokenInfo> tmpres = await _authBLL.CheckToken(this.IntentAction);
            if (!string.IsNullOrEmpty(permis) && tmpres.IsSuccess())
            {
                BusResponse<Data_ServerTokenInfo> response = await _authBLL.CheckPermis(tmpres.Data, permis);
                return response.ToAjaxResult();
            }
            return tmpres.ToAjaxResult();
        }

        /// <summary>
        /// 解释邀请码
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet]
        [TANetWebApiResponse(typeof(DefaultAjaxResult<Out_InvitData>))]
        public async Task<AjaxResult> InviteCode(string code)
        {
            return (await _orgBLL.ParseInvitCode(code)).ToAjaxResult();
        }
    }
}
