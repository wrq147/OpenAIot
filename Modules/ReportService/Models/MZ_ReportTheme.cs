using Common.Share;
using MyAccess.DB.Attr;
using System;

namespace ReportService.Models
{
    /// <summary>
    /// 报表主题
    /// </summary>
    [TableName("mz_report_theme")]
    public class MZ_ReportTheme : BaseEntity
    {
        /// <summary>
        /// 编码Id
        /// </summary>
        [ID(false)]
        public string Id { get; set; }
        /// <summary>
        /// 主题名称
        /// </summary>
        public string ThemeName { get; set; }
        /// <summary>
        /// 主题配置json
        /// </summary>
        public string ThemeOption { get; set; }
    }
}
