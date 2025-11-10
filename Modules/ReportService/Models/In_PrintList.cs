using Common.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportService.Models
{
    /// <summary>
    /// 过滤打印模板列表
    /// </summary>
    public class In_PrintList : BaseQueryParam
    {
        /// <summary>
        /// 过滤数据源
        /// </summary>
        public string DataId { get; set; }
    }
}
