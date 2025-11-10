using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailService.Model
{
    public class In_LoginEmail
    {
        /// <summary>
        /// 邮箱
        /// </summary>
        public string email { get; set; }
        /// <summary>
        /// 邮箱验证码
        /// </summary>
        public string code { get; set; }
    }
}
