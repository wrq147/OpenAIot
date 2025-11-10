using AuthService;
using AuthService.Controller;
using EmailService.Business;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Route;

namespace EmailService.Controller
{
    /// <summary>
    /// 邮箱绑定API
    /// </summary>
    public class Email : AbstractLoginedController
    {
        private EmailBLL _emailBLL;
        private ConfigBLL _configBLL;
        public Email(EmailBLL emailBLL, ConfigBLL configBLL)
        {
            _emailBLL = emailBLL;
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
            string emailcont = await _configBLL.SelectConfigByKey("sys.email.sendTemplate");
            return (await _emailBLL.SendCode(email, emailcont, GetUser())).ToAjaxResult();
        }
        /// <summary>
        /// 绑定邮箱
        /// </summary>
        /// <param name="note"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> BindEmail(string note)
        {
            var user = GetUser();
            return (await _emailBLL.BindEmail(note, user.UserId)).ToAjaxResult();
        }
    }
}
