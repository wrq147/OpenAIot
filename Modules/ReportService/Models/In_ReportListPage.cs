using Common.Share;
using System;

namespace ReportService.Models
{
    public class In_ReportListPage : BaseQueryParam
    {
        /// <summary>
        /// 过滤组织（前端不用传）
        /// </summary>
        public long? OrgId { get; set; }
        /// <summary>
        /// 关键词搜索
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 状态（0正常 1停用 2已发布）
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 分组Id
        /// </summary>
        public string GroupId { get; set; }
    }
}
