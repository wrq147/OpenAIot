using Common.Attr;
using MyAccess.DB.Attr;
using Newtonsoft.Json;
using ProducerService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESService.Model
{
    /// <summary>
    /// 生产任务
    /// </summary>
    [TableName("mz_work_task")]
    public class MZ_WorkTask
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
        /// 关联的工单Id
        /// </summary>
        public string WorkOrderId { get; set; }
        /// <summary>
        /// 所属产品ID
        /// </summary>
        public string ProductId { get; set; }
        /// <summary>
        /// 关联的工序Id
        /// </summary>
        public string OperId { get; set; }
        /// <summary>
        /// 关联的工艺路线明细Id
        /// </summary>
        public string RouteOperId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreatedOn { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartOn { get; set; }
        /// <summary>
        /// 完成时间
        /// </summary>
        public DateTime? FinishOn { get; set; }
        /// <summary>
        /// 任务是否完成
        /// </summary>
        public bool? IsFinish { get; set; }
        /// <summary>
        /// 优先级：1、优先安排；2、加急处理；3、正常排产
        /// </summary>
        public int? Priority { get; set; }
        /// <summary>
        /// 超期时间
        /// </summary>
        public DateTime? OverTime { get; set; }
        /// <summary>
        /// 报工数配比
        /// </summary>
        public decimal? PropOf { get; set; }
        /// <summary>
        /// 工时(分钟)
        /// </summary>
        public decimal? WorkTime { get; set; }
        /// <summary>
        /// 总工时(分钟)
        /// </summary>
        public decimal? WorkTimeTotal { get; set; }
        /// <summary>
        /// 计划数
        /// </summary>
        public decimal? PlanNum { get; set; }
        /// <summary>
        /// 良品数
        /// </summary>
        public decimal? GoodNum { get; set; }
        /// <summary>
        /// 不良品数
        /// </summary>
        public decimal? DefectNum { get; set; }
        /// <summary>
        /// 工序顺序序号
        /// </summary>
        public int? Sequence { get; set; }
        /// <summary>
        /// 任务说明
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 工单编号
        /// </summary>
        [DataIgnore]
        public string WorkNumber { get; set; }
        /// <summary>
        /// 工序名称
        /// </summary>
        [DataIgnore]
        public string OperName { get; set; }
        /// <summary>
        /// 允许提交的人员
        /// </summary>
        [DataIgnore]
        public string AssignedUser { get; set; }
        /// <summary>
        /// 工艺信息
        /// </summary>
        [DataIgnore]
        public MZ_ProductRouteOper RouteOper { get; set; }
        /// <summary>
        /// 产品编号
        /// </summary>
        [DataIgnore]
        public string SkuNumber { get; set; }
        /// <summary>
        /// 产品名称
        /// </summary>
        [DataIgnore]
        public string ProductName { get; set; }
    }
}
