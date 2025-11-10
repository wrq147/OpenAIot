using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeiXinService.Model
{
    public class In_GenerateWxSchemeTick
    {
        /// <summary>
        /// 跳转的小程序appid
        /// </summary>
        public string appid { get; set; }
        /// <summary>
        /// 跳转的小程序path
        /// </summary>
        public string path { get; set; }
        /// <summary>
        /// 跳转的小程序query
        /// </summary>
        public string query { get; set; }
    }
}
