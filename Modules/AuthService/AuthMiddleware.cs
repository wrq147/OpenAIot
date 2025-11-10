using Common.Share;
using Microsoft.Extensions.Options;
using System;
using TemplateAction.Core;
using TemplateAction.Label;
using System.Threading.Tasks;
using Common;
using System.Net.Http;

namespace AuthService
{
    public class AuthMiddleware : IFilterMiddleware
    {
        private IOptions<GeneralOption> _conf;
        public AuthMiddleware(IOptions<GeneralOption> conf)
        {
            _conf = conf;
        }
        public async Task<IResult> Excute(TAAction ac, FilterMiddlewareNode next)
        {
            bool canAuth = false;
            if (typeof(ILoginController).IsAssignableFrom(ac.ControllerNode.ControllerType))
            {
                canAuth = true;
            }
            if (canAuth)
            {
                //需要身份认证
                Data_ServerTokenInfo dsti = null;
                //判断是否设置了认证中心
                if (string.IsNullOrEmpty(_conf.Value.token_url))
                {
                    var authBLL = ac.Context.Application.ServiceProvider.GetService<AuthBLL>();
                    BusResponse<Data_ServerTokenInfo> tmpres = await authBLL.CheckToken(ac);
                    if (!tmpres.IsSuccess())
                    {
                        return tmpres.ToAjaxResult();
                    }
                    dsti = tmpres.Data;
                    BusResponse<Data_ServerTokenInfo> response = await authBLL.CheckPermis(dsti, ac.ActionNode.AboutCode);
                    if (!response.IsSuccess())
                    {
                        return response.ToAjaxResult();
                    }
                }
                else
                {
                    //权限中心认证
                    string stt = ac.Context.Request.Header[AuthConstant.CONFIG_TOKEN_KEY];
                    var httpReq = new HttpRequestMessage(HttpMethod.Get, _conf.Value.token_url + "?permis=" + Uri.EscapeDataString(ac.ActionNode.AboutCode));
                    var userAgentInfo = ac.Context.Request.Header["User-Agent"];
                    httpReq.Headers.TryAddWithoutValidation("User-Agent", userAgentInfo);
                    httpReq.Headers.TryAddWithoutValidation(AuthConstant.CONFIG_TOKEN_KEY, stt);

                    string rt = await HttpHelper.Instance.SendAsync(httpReq);
                    AuthServerResult tokenrt = Newtonsoft.Json.JsonConvert.DeserializeObject<AuthServerResult>(rt);
                    if (tokenrt.code != Constants.SUCCESS_CODE)
                    {
                        return new DefaultAjaxResult<string>(tokenrt.code, tokenrt.message);
                    }
                    else
                    {
                        dsti = tokenrt.data;
                        dsti.SetRemoteInfo(stt, _conf.Value.scope_url, userAgentInfo);
                    }
                }

                Data_ServerTokenInfo.To(ac.Context, dsti);
                return await next.Excute(ac);
            }
            else
            {
                //不需要认证
                return await next.Excute(ac);
            }
        }
    }
}
