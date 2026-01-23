using Common.Attr;
using MyAccess.DB.Attr;
using ProducerService.Model;

namespace MESService.Model
{
    /// <summary>
    /// 物料清单明细表
    /// </summary>
    [TableName("mz_bom_line")]
    public class MZ_BomLine
    {
        /// <summary>
        /// 编码
        /// </summary>
        [ID(false)]
        public string Id { get; set; }
        /// <summary>
        /// 所属组织ID
        /// </summary>
        [OnlySeriaize]
        public long? OrgId { get; set; }
        /// <summary>
        /// 所属BOM头Id
        /// </summary>
        [OnlySeriaize]
        public string HeaderId { get; set; }
        /// <summary>
        /// 父项产品Id
        /// </summary>
        [OnlySeriaize]
        public string ParentProductId { get; set; }
        /// <summary>
        /// 子项产品Id
        /// </summary>
        public string ProductId { get; set; }
        /// <summary>
        /// 排序值：越小越前面
        /// </summary>
        public int? Sort { get; set; }
        /// <summary>
        /// 使用的计量
        /// </summary>
        public decimal? Quantity { get; set; }
        /// <summary>
        /// 关联工序ID
        /// </summary>
        public string ProcessStepId { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 子项产品信息
        /// </summary>
        [DataIgnore]
        public MZ_Product ProInfo { get; set; }
        /// <summary>
        /// 关联工序信息
        /// </summary>
        [DataIgnore]
        public MZ_ProductOper ProcessStepInfo { get; set; }
    }
}
