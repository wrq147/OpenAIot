using Common.Attr;
using Common.Share;
using MyAccess.DB.Attr;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;


namespace MESService.Model
{
    /// <summary>
    /// 生产计划
    /// </summary>
    [TableName("mz_product_plan")]
    public class MZ_ProductPlan : BaseEntity
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
        /// 唯一编号
        /// </summary>
        public string Number { get; set; }
        /// <summary>
        /// 计划名称
        /// </summary>
        public string PlanName { get; set; }
        /// <summary>
        /// 状态：0、待提交；1、待审批；2、待执行；3、执行中；4、已完成；5、已取消、6、已驳回
        /// </summary>
        public int? Status { get; set; }
        /// <summary>
        /// 优先级：1、优先安排；2、加急处理；3、正常排产
        /// </summary>
        public int? Priority { get; set; }
        /// <summary>
        /// 关联的审核流程Id
        /// </summary>
        public long? FlowId { get; set; }
        /// <summary>
        /// 超期时间
        /// </summary>
        public DateTime? OverTime { get; set; }


        /// <summary>
        /// 生产计划的产品信息
        /// </summary>
        [DataIgnore]
        public List<MZ_ProductPlanItem> Items { get; set; }
    }
}
