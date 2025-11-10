using System;
using TemplateAction.Label;
using Common;
using TemplateAction.NetCore;
using System.Threading.Tasks;
using TemplateAction.Core;
using Newtonsoft.Json;
using Common.Share;

namespace AuthService.Controller
{
    /// <summary>
    /// 已登录接口处理需要继承此控制器
    /// </summary>
    public abstract class AbstractLoginedController: TANetController, ILoginController
    {
        /// <summary>
        /// 异常处理
        /// </summary>
        /// <param name="code"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        public override IResult Exception(int code, Exception ex)
        {
            Context.Items["OperException"] = ex;
            return this.Error<string>(code, ex.Message);
        }
        public override async Task<IResult> CallAction(TAAction ac, object[] parameters)
        {
            if (parameters != null && parameters.Length > 0)
            {
                //存储json格式参数
                Context.Items["OperParam"] = parameters;
            }
            return await base.CallAction(ac, parameters);
        }
        /// <summary>
        /// 获取当前登录用户
        /// </summary>
        /// <returns></returns>
        protected Data_ServerTokenInfo GetUser()
        {
            return Data_ServerTokenInfo.From(Context);
        }
    }
}
