using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeiXinService.Model
{
    /// <summary>
    /// 企业微信登录参数
    /// </summary>
    public class In_LoginCorp
    {
        /// <summary>
        /// 登录的AppId
        /// </summary>
        public string appid { get; set; }
        /// <summary>
        /// 登录code
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 是否不存在自动创建账号
        /// </summary>
        public bool? created { get; set; }
        /// <summary>
        /// 扩展信息
        /// </summary>
        public Dictionary<string, string> ext { get; set; }
    }
}
