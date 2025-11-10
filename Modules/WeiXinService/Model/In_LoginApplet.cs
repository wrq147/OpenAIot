using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeiXinService.Model
{
    /// <summary>
    /// 微信登录参数
    /// </summary>
    public class In_LoginApplet
    {
        /// <summary>
        /// 登录的AppId
        /// </summary>
        public string appid { get; set; }
        /// <summary>
        /// 小程序登录code
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 是否不存在自动创建账号
        /// </summary>
        public bool? created { get; set; }

    }
}
