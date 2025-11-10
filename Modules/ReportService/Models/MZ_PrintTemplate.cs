using Common.Share;
using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportService.Models
{
    [TableName("mz_print_template")]
    public class MZ_PrintTemplate : BaseEntity
    {
        /// <summary>
        /// 模板ID
        /// </summary>
        [ID(false)]
        public string Id { get; set; }
        /// <summary>
        /// 所属组织ID
        /// </summary>
        public long? OrgId { get; set; }
        /// <summary>
        /// 模板名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 模板内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 纸张方向
        /// </summary>
        public string PaperDirection { get; set; }
        /// <summary>
        /// 页面上边距
        /// </summary>
        public int? PaperMarginTop { get; set; }
        /// <summary>
        /// 页面下边距
        /// </summary>
        public int? PaperMarginBottom { get; set; }
        /// <summary>
        /// 背景颜色
        /// </summary>
        public string Background { get; set; }
        /// <summary>
        /// 默认字体
        /// </summary>
        public string FontFamily { get; set; }
        /// <summary>
        /// 默认行高
        /// </summary>
        public int? LineHeight { get; set; }
        /// <summary>
        /// 纸张类型名
        /// </summary>
        public string PaperName { get; set; }
        /// <summary>
        /// 纸张宽
        /// </summary>
        public int? PaperWidth { get; set; }
        /// <summary>
        /// 纸张高
        /// </summary>
        public int? PaperHeight { get; set; }
        /// <summary>
        /// 数据源Id
        /// </summary>
        public string DataId { get; set; }
    }
}
