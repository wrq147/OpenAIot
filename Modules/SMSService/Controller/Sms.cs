using AuthService;
using AuthService.Controller;
using SMSService.Business;
using SMSService.Model;
using System;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Route;

namespace SMSService.Controller
{
    public class Sms : AbstractLoginedController
    {
        private SmsCodeBLL _smsBLL;
        public Sms(SmsCodeBLL sms)
        {
            _smsBLL = sms;
        }

        /// <summary>
        /// 修改绑定手机
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<AjaxResult> Bind(string phone, string code)
        {
            return (await _smsBLL.Bind(phone, code, GetUser())).ToAjaxResult();
        }

    }
}
