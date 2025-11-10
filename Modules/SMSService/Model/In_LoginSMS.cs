using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSService.Model
{
    public class In_LoginSMS
    {
        /// <summary>
        /// 手机号
        /// </summary>
        public string tel { get; set; }
        /// <summary>
        /// 短信验证码
        /// </summary>
        public string code { get; set; }
    }
}
