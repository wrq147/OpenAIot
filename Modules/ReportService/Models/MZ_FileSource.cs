using Common.Share;
using MyAccess.DB.Attr;
using System;
namespace ReportService.Models
{
    /// <summary>
    /// 文件数据源
    /// </summary>
    [TableName("mz_file_source")]
    public class MZ_FileSource : BaseEntity
    {
        /// <summary>
        /// 编码Id
        /// </summary>
        [ID(false)]
        public string Id { get; set; }
        /// <summary>
        /// 所属组织ID
        /// </summary>
        public long? OrgId { get; set; }
        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 文件连接
        /// </summary>
        public string FileUrl { get; set; }
        /// <summary>
        /// 解析后的表格json数据
        /// </summary>
        public string FileData { get; set; }
        /// <summary>
        /// 图表类型
        /// </summary>
        public string ChartType { get; set; }
    }
}
