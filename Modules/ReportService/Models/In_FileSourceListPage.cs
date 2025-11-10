using Common.Share;
using System;

namespace ReportService.Models
{
    public class In_FileSourceListPage : BaseQueryParam
    {
        /// <summary>
        /// 过滤文件名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 过滤所属企业
        /// </summary>
        public long? OrgId { get; set; }
    }
}
