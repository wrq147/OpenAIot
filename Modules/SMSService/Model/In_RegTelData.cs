using AuthService.Model;
using System;

namespace SMSService.Model
{
    public class In_RegTelData: In_RegData
    {
        /// <summary>
        /// 手机验证码
        /// </summary>
        public string MobileCode { get; set; }
    }
}
