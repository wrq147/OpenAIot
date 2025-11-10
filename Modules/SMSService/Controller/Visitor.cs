using TemplateAction.Core;
using TemplateAction.NetCore;
using Common;
using TemplateAction.Route;
using System.Threading.Tasks;
using System;
using SMSService.Business;
using AuthService;
using SMSService.Model;
using Common.Share;

namespace SMSService.Controller
{
    /// <summary>
    /// 短信服务
    /// </summary>
    public class Visitor : TANetController
    {
        private SmsCodeBLL _smsBLL;
        public Visitor(SmsCodeBLL smsBLL)
        {
            _smsBLL = smsBLL;
        }
        /// <summary>
        /// 生成短信发送验证码
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> GetSMSCaptcha()
        {
            CaptchaImageHelper securityCode = this.ServiceProvider.GetService<CaptchaImageHelper>();
            string code = securityCode.GetRandomEnDigitalText(4);
            byte[] bytes = securityCode.GetEnDigitalCodeByte(code);

            return this.Success(new
            {
                uuid = await _smsBLL.MakeImgToken(code),
                base64 = "data:image/png;base64," + Convert.ToBase64String(bytes)
            });
        }
        /// <summary>
        /// 发送短信验证码,用来判断手机号码是否存在
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="code"></param>
        /// <param name="imgid"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> SendCode(string phone, string code = "", string imgid = "")
        {
            return (await _smsBLL.SendCode(phone, code, imgid, Context)).ToAjaxResult();
        }
        /// <summary>
        /// 注册用户（需要手机验证码）
        /// </summary>
        /// <param name="data"></param>
        /// <returns>返回登录的信息</returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<Out_Login>> Reg(In_RegTelData data)
        {
            return (await _smsBLL.Reg(data, Context)).ToAjaxResult();
        }
        /// <summary>
        /// 手机短信登录
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<Out_Login>> FromTel(In_LoginSMS data)
        {
            BusResponse<Out_Login> response = await _smsBLL.Login(data, Context);
            return response.ToAjaxResult();
        }
    }
}
