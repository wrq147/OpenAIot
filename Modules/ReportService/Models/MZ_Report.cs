using Common.Attr;
using Common.Share;
using MyAccess.DB.Attr;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace ReportService.Models
{
    /// <summary>
    /// 报表存储结构
    /// </summary>
    [TableName("mz_report")]
    public class MZ_Report : BaseEntity
    {
        /// <summary>
        /// 编码Id
        /// </summary>
        [ID(false)]
        public string Id { get; set; }
        /// <summary>
        /// 报表名称
        /// </summary>
        [Required(ErrorMessage = "报表名称不能为空")]
        public string Name { get; set; }
        /// <summary>
        /// 报表类型：screen、table
        /// </summary>
        public string ReportType { get; set; }
        /// <summary>
        /// 所属组织ID
        /// </summary>
        public long? OrgId { get; set; }
        /// <summary>
        /// 分组Id
        /// </summary>
        public string GroupId { get; set; }
        /// <summary>
        /// 报表描述
        /// </summary>
        public string DesInfo { get; set; }
        /// <summary>
        /// 标签
        /// </summary>
        public string Tag { get; set; }
        /// <summary>
        /// 大屏分辨率
        /// </summary>
        public string Resolution { get; set; }
        /// <summary>
        /// 设备类型 pc、phone、double
        /// </summary>
        public string DeviceType { get; set; }
        /// <summary>
        /// 组件累计Id
        /// </summary>
        public int? IdGlobal { get; set; }
        /// <summary>
        /// 数据大屏再画数据结构
        /// </summary>
        public string DrawOption { get; set; }
        /// <summary>
        /// 主题再画数据结构
        /// </summary>
        public string ThemeOption { get; set; }
        /// <summary>
        /// echart图option集合
        /// </summary>
        public string MapOption { get; set; }
        /// <summary>
        /// 全局累计zindex图层索引
        /// </summary>
        public int? Zindex { get; set; }
        /// <summary>
        /// 报表缩略图
        /// </summary>
        [JsonConverter(typeof(ImageUrl))]
        public string Thumbnail { get; set; }
        /// <summary>
        /// 状态（0未发布 2已发布）
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 删除标志（0代表存在 2代表删除）
        /// </summary>
        public string del_flag { get; set; }

    }
}
