using Common.Attr;
using MyAccess.DB.Attr;
using Newtonsoft.Json;
using ProducerService.Model;
using System;

namespace MESService.Model
{
    /// <summary>
    /// 生产工单
    /// </summary>
    [TableName("mz_work_order")]
    public class MZ_WorkOrder
    {
        /// <summary>
        /// Id编号
        /// </summary>
        [ID(false)]
        public string Id { get; set; }
        /// <summary>
        /// 所属组织ID
        /// </summary>
        [JsonConverter(typeof(OnlySeriaize))]
        public long? OrgId { get; set; }
        /// <summary>
        /// 生产计划Id
        /// </summary>
        public string PlanId { get; set; }
        /// <summary>
        /// 工单编号
        /// </summary>
        public string WorkNumber { get; set; }
        /// <summary>
        /// 生产计划名称
        /// </summary>
        [DataIgnore]
        public string PlanName { get; set; }
        /// <summary>
        /// 父工单Id
        /// </summary>
        public string ParentWorkOrderId { get; set; }

        /// <summary>
        /// 状态：0、待生产；1、生产中；2、已完成；3、已取消；
        /// </summary>
        public int? Status { get; set; }
        /// <summary>
        /// 优先级：1、优先安排；2、加急处理；3、正常排产
        /// </summary>
        public int? Priority { get; set; }
        /// <summary>
        /// 超期时间
        /// </summary>
        public DateTime? OverTime { get; set; }
        /// <summary>
        /// 计划产量
        /// </summary>
        public decimal? Quantity { get; set; }
        /// <summary>
        /// 所属产品ID
        /// </summary>
        public string ProductId { get; set; }
        /// <summary>
        /// 工艺路线Id
        /// </summary>
        public string RouteId { get; set; }
        /// <summary>
        /// 计划开始时间
        /// </summary>
        public DateTime? PlannedStartOn { get; set; }
        /// <summary>
        /// 计划结束时间
        /// </summary>
        public DateTime? PlannedEndOn { get; set; }
        /// <summary>
        /// 实际开始时间
        /// </summary>
        public DateTime? StartOn { get; set; }
        /// <summary>
        /// 实际结束时间
        /// </summary>
        public DateTime? EndOn { get; set; }
        /// <summary>
        /// 取消原因
        /// </summary>
        public string CancelReason { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreatedOn { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdatedOn { get; set; }

        /// <summary>
        /// 产品信息
        /// </summary>
        [DataIgnore]
        public MZ_Product ProdInfo { get; set; }
        /// <summary>
        /// 父工单信息
        /// </summary>
        [DataIgnore]
        public Out_ParentWordInfo ParentWorkInfo { get; set; }
    }
}
