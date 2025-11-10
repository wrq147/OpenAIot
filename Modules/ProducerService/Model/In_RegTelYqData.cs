using AuthService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProducerService.Model
{
    public class In_RegTelYqData : In_RegData
    {
        /// <summary>
        /// 手机邀请验证码
        /// </summary>
        public string MobileCode { get; set; }
        /// <summary>
        /// 手机邀请码
        /// </summary>
        public string YqCode { get; set; }
    }
}
