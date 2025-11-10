using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeiXinService.Model
{
    public class In_YqSms
    {
        /// <summary>
        /// 微信AppId
        /// </summary>
        public string appid { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string tel { get; set; }
        /// <summary>
        /// 邀请码
        /// </summary>
        public string tk { get; set; }
    }
}
