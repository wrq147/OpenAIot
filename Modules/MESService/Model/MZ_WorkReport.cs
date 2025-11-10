using Common.Attr;
using Common.Share;
using MyAccess.DB.Attr;
using Newtonsoft.Json;
using System;


namespace MESService.Model
{
    /// <summary>
    /// 报工
    /// </summary>
    [TableName("mz_work_report")]
    public class MZ_WorkReport : BaseEntity
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
        /// 关联的工单Id
        /// </summary>
        public string WorkOrderId { get; set; }
        /// <summary>
        /// 关联的任务Id
        /// </summary>
        public string WorkTaskId { get; set; }
        /// <summary>
        /// 关联的工序Id
        /// </summary>
        public string OperId { get; set; }
        /// <summary>
        /// 唯一编号
        /// </summary>
        public string Number { get; set; }
        /// <summary>
        /// 批次编号
        /// </summary>
        public string BatchNo { get; set; }
        /// <summary>
        /// 良品数
        /// </summary>
        public decimal? GoodNum { get; set; }
        /// <summary>
        /// 不良品数
        /// </summary>
        public decimal? DefectNum { get; set; }
        /// <summary>
        /// 不良品项：多个逗号分隔
        /// </summary>
        public string DefectStr { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartWork { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndWork { get; set; }
        /// <summary>
        /// 报工时长(分钟)
        /// </summary>
        public decimal? WorkTime { get; set; }
        /// <summary>
        /// 超时原因，当报工时长超过标准时间时需要填写
        /// </summary>
        public string OverReason { get; set; }
        /// <summary>
        /// 状态：0、待提交；1、待审核；2、已审核；3、已取消；4、已驳回；
        /// </summary>
        public int? Status { get; set; }
        /// <summary>
        /// 关联的审核流程Id
        /// </summary>
        public long? FlowId { get; set; }

        /// <summary>
        /// 报工的批次
        /// </summary>
        [DataIgnore]
        public MZ_WorkBatch RepBat { get; set; }
    }
}
