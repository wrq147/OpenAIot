using Common.Share;
using System;


namespace ReportService.Models
{
    public class In_ShareListPage : BaseQueryParam
    {
        /// <summary>
        /// 通过报表名称搜索
        /// </summary>
        public string SearchKey { get; set; }
        /// <summary>
        /// 过滤报表Id
        /// </summary>
        public string ReportId { get; set; }
    }
}
