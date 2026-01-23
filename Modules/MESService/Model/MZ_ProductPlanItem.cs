using Common.Attr;
using MyAccess.DB.Attr;
using ProducerService.Model;
using System;

namespace MESService.Model
{
    /// <summary>
    /// 生产计划的产品信息
    /// </summary>
    [TableName("mz_product_plan_item")]
    public class MZ_ProductPlanItem
    {
        /// <summary>
        /// Id编号
        /// </summary>
        [ID(false)]
        public string Id { get; set; }
        /// <summary>
        /// 所属组织ID
        /// </summary>
        [OnlySeriaize]
        public long? OrgId { get; set; }
        /// <summary>
        /// 生产计划Id
        /// </summary>
        public string PlanId { get; set; }
        /// <summary>
        /// 所属产品ID
        /// </summary>
        public string ProductId { get; set; }
        /// <summary>
        /// 计划数量
        /// </summary>
        public decimal? Quantity { get; set; }
        /// <summary>
        /// 计划开始时间
        /// </summary>
        public DateTime? PlannedStartOn { get; set; }
        /// <summary>
        /// 计划结束时间
        /// </summary>
        public DateTime? PlannedEndOn { get; set; }

        /// <summary>
        /// 产品信息
        /// </summary>
        [DataIgnore]
        public MZ_Product ProdInfo { get; set; }
    }
}
