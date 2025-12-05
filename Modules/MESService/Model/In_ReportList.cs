using AuthService.Fields;
using Common.Share;
using System;

namespace MESService.Model
{
    public class In_ReportList : BaseQueryParam
    {
        /// <summary>
        /// 过滤指定任务
        /// </summary>
        public string TaskId { get; set; }
        /// <summary>
        /// 是否只显示我的报工
        /// </summary>
        public bool? OnlyMy { get; set; }
        /// <summary>
        /// 过滤报工人员
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 过滤指定批次编号
        /// </summary>
        public string BatchNo { get; set; }
        /// <summary>
        /// 过滤扩展字段
        /// </summary>
        public FieldFilterItem[] Items { get; set; }
    }
}
