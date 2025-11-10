using AuthService;
using AuthService.Model;
using Common.Share;
using EmailService.Business;
using EmailService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.NetCore;
using TemplateAction.Route;

namespace EmailService.Controller
{
    /// <summary>
    /// 游客执行
    /// </summary>
    public class Visitor : TANetController
    {
        private EmailBLL _emailBLL;
        private ConfigBLL _configBLL;
        public Visitor(EmailBLL email, ConfigBLL configBLL)
        {
            _emailBLL = email;
            _configBLL = configBLL;
        }
        /// <summary>
        /// 发送验证码邮件
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> SendCode(string email)
        {
            string emailcont = await _configBLL.SelectConfigByKey("sys.email.validTemplate");
            return (await _emailBLL.SendCode(email, emailcont, null)).ToAjaxResult();
        }
        /// <summary>
        /// 激活邮箱
        /// </summary>
        /// <param name="note">激活码</param>
        /// <param name="uid">用户id</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ViewResult> ActiveEmail(string note, long uid)
        {
            var rt = await _emailBLL.BindEmail(note, uid);
            if (rt.IsSuccess())
            {
                return new ViewResult(@"<html><header><meta charset='UTF-8'></header><body style='text-align:center;font-size: 20px; font-weight: bold;'>激活成功！</body></html>");
            }
            else
            {
                return new ViewResult($"<html><header><meta charset='UTF-8'></header><body style='text-align:center;font-size: 20px; font-weight: bold;'>激活失败,{rt.Message}！</body></html>");
            }
        }

        /// <summary>
        /// 注册用户
        /// </summary>
        /// <param name="data"></param>
        /// <returns>返回登录的信息</returns>
        [HttpPost]
        [TANetWebApiResponse(typeof(DefaultAjaxResult<Out_Login>))]
        public async Task<AjaxResult> Reg(In_RegData data)
        {
            return (await _emailBLL.Reg(data, Context)).ToAjaxResult();
        }

        /// <summary>
        /// 邮箱验证码登录
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<Out_Login>> FromEmail(In_LoginEmail data)
        {
            BusResponse<Out_Login> response = await _emailBLL.Login(data, Context);
            return response.ToAjaxResult();
        }
    }
}
