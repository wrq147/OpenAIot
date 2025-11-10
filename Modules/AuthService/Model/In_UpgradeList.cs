using Common.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Model
{
    public class In_UpgradeList : BaseQueryParam
    {
        /// <summary>
        /// 搜索版本、标题、内容
        /// </summary>
        public string SearchKey { get; set; }
        /// <summary>
        /// 过滤应用所属
        /// </summary>
        public long? OrgId { get; set; }
        /// <summary>
        /// 过滤主题Id
        /// </summary>
        public string StyleId { get; set; }
    }
}
