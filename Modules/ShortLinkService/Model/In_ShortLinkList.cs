using Common.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShortLinkService.Model
{
    public class In_ShortLinkList : BaseQueryParam
    {
        /// <summary>
        /// 搜索关键词
        /// </summary>
        public string Key { get; set; }
    }
}
