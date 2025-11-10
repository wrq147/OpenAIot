using Common.Share;
using MyAccess.DB.Attr;
using System;

namespace ReportService.Models
{
    /// <summary>
    /// 报表组件
    /// </summary>
    [TableName("mz_report_com")]
    public class MZ_ReportCom : BaseEntity
    {
        /// <summary>
        /// 编码Id
        /// </summary>
        [ID(false)]
        public string Id { get; set; }
        /// <summary>
        /// 组件名称（唯一）
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 所属组织ID
        /// </summary>
        public long? OrgId { get; set; }
        /// <summary>
        /// 组件的搜索关键字
        /// </summary>
        public string Tag { get; set; }
        /// <summary>
        /// 组件option配置
        /// </summary>
        public string Option { get; set; }
        /// <summary>
        /// 所属图表
        /// </summary>
        public string Graph { get; set; }
        /// <summary>
        /// 所属组件
        /// </summary>
        public string Component { get; set; }
        /// <summary>
        /// 组件描述
        /// </summary>
        public string DesInfo { get; set; }
        /// <summary>
        /// 缩略图
        /// </summary>
        public string Thumbnail { get; set; }
        /// <summary>
        /// 状态（0正常 1停用 2已发布）
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 删除标志（0代表存在 2代表删除）
        /// </summary>
        public string del_flag { get; set; }
    }
}
