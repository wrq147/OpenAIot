using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeiXinService.Model
{
    public class In_WxAppletBind
    {
        /// <summary>
        /// 登录的AppId
        /// </summary>
        public string appid { get; set; }
        /// <summary>
        /// 小程序登录code
        /// </summary>
        public string code { get; set; }
    }
}
