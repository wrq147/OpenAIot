using Common.Share;
using System;
using System.ComponentModel.DataAnnotations;

namespace CardService.Model
{
    /// <summary>
    /// 产品查询
    /// </summary>
    public class In_Card_Pro : BaseQueryParam
    {
        /// <summary>
        /// 搜索的关键字
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 过滤分类
        /// </summary>
        public long? CategoryId { get; set; }
        /// <summary>
        /// 过滤分类Path
        /// </summary>
        public string CategoryPath { get; set; }
        /// <summary>
        /// 过滤名片
        /// </summary>
        public long? CardId { get; set; }
        /// <summary>
        /// 过滤组织
        /// </summary>
        public long? orgId { get; set; }
        /// <summary>
        /// 删除标记
        /// </summary>
        public string del_flag { get; set; }
        /// <summary>
        /// 过滤部门
        /// </summary>
        public string filterAncestors { get; set; }
    }
}
