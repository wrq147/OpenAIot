using Common.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Model
{
    public class In_StyleOrgList : BaseQueryParam
    {
        /// <summary>
        /// 指定主题
        /// </summary>
        public string StyleId { get; set; }
        /// <summary>
        /// 是否过滤已存在主题的企业
        /// </summary>
        public bool? NoExist { get; set; }
        /// <summary>
        /// 搜索关键字
        /// </summary>
        public string Key { get; set; }
    }
}
