using System;
using System.ComponentModel.DataAnnotations;

namespace CardService.Model
{
    /// <summary>
    /// 产品分类查询
    /// </summary>
    public class In_Card_Category
    {
        /// <summary>
        /// 搜索的关键字
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 过滤组织
        /// </summary>
        public long? OrgId { get; set; }
        /// <summary>
        /// 过滤父分类
        /// </summary>
        public long? ParentId { get; set; }
        /// <summary>
        /// 过滤名片
        /// </summary>
        public long? CardId { get; set; }
    }
}
