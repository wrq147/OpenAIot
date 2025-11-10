using Common.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportService.Models
{
    public class In_ApiSourceListPage : BaseQueryParam
    {
        /// <summary>
        /// 过滤接口名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 过滤接口类型
        /// </summary>
        public string ApiType { get; set; }
    }
}
