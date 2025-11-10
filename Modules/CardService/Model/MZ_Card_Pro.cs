using Common.Attr;
using Common.Share;
using MyAccess.DB.Attr;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace CardService.Model
{
    /// <summary>
    /// 产品实体
    /// </summary>
    [TableName("mz_card_pro")]
    public class MZ_Card_Pro : BaseEntity
    {
        /// <summary>
        /// 编号
        /// </summary>
        [ID(false)]
        public long? Id { get; set; }
        /// <summary>
        /// 所属组织Id
        /// </summary>
        [JsonConverter(typeof(OnlySeriaize))]
        public long? OrgId { get; set; }
        /// <summary>
        /// 所属部门Id
        /// </summary>
        public long? DeptId { get; set; }
        /// <summary>
        /// 产品主图
        /// </summary>
        public string ImageUrl { get; set; }
        /// <summary>
        /// 产品名称
        /// </summary>
        [Required(ErrorMessage = "产品名称不能为空")]
        public string ProName { get; set; }
        /// <summary>
        /// 产品类别Id
        /// </summary>
        [Required(ErrorMessage = "产品类别不能为空")]
        public long? CategoryId { get; set; }
        /// <summary>
        /// 单价
        /// </summary>
        [Required(ErrorMessage = "单价不能为空")]
        public decimal? Price { get; set; }
        /// <summary>
        /// 批发价
        /// </summary>
        [Required(ErrorMessage = "批发价不能为空")]
        public decimal? WholePrice { get; set; }
        /// <summary>
        /// 产品详情（json数组[{"t":"img","v":"https://xx/g.jpg"}])
        /// </summary>
        [Required(ErrorMessage = "产品详情不能为空")]
        public string Detail { get; set; }

        /// <summary>
        /// 删除标志（0代表存在 2代表删除）
        /// </summary>
        [JsonIgnore]
        public string del_flag { get; set; }
    }
}
