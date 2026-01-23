using Common.Attr;
using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CardService.Model
{
    /// <summary>
    /// 产品分类实体
    /// </summary>
    [TableName("mz_card_procat")]
    public class MZ_Card_Category
    {
        /// <summary>
        /// 编号
        /// </summary>
        [ID(false)]
        public long? Id { get; set; }
        /// <summary>
        /// 所属组织Id
        /// </summary>
        [OnlySeriaize]
        public long? OrgId { get; set; }
        /// <summary>
        /// 分类名称
        /// </summary>
        [Required(ErrorMessage = "分类名称不能为空")]
        public string CategoryName { get; set; }
        /// <summary>
        /// 分类排序
        /// </summary>
        public int? Sort { get; set; }
        /// <summary>
        /// 父分类Id
        /// </summary>
        public long? ParentId { get; set; }
        /// <summary>
        /// 分类层级
        /// </summary>
        [JsonIgnore]
        public string Path { get; set; }

        /// <summary>
        /// 子分类
        /// </summary>
        [DataIgnore]
        public List<MZ_Card_Category> children { get; set; }
    }
}
