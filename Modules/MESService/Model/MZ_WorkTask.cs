using Common.Attr;
using MyAccess.DB.Attr;
using Newtonsoft.Json;
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
        /// 关联的工序Id
        /// </summary>
        public string OperId { get; set; }
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
        /// 扩展字符串字段1
        /// </summary>
        public string StrExt1 { get; set; }
        /// <summary>
        /// 扩展字符串字段2
        /// </summary>
        public string StrExt2 { get; set; }
        /// <summary>
        /// 扩展字符串字段3
        /// </summary>
        public string StrExt3 { get; set; }
        /// <summary>
        /// 扩展字符串字段4
        /// </summary>
        public string StrExt4 { get; set; }
        /// <summary>
        /// 扩展字符串字段5
        /// </summary>
        public string StrExt5 { get; set; }
        /// <summary>
        /// 扩展字符串字段6
        /// </summary>
        public string StrExt6 { get; set; }
        /// <summary>
        /// 扩展字符串字段7
        /// </summary>
        public string StrExt7 { get; set; }
        /// <summary>
        /// 扩展字符串字段8
        /// </summary>
        public string StrExt8 { get; set; }
        /// <summary>
        /// 扩展字符串字段9
        /// </summary>
        public string StrExt9 { get; set; }
        /// <summary>
        /// 扩展字符串字段10
        /// </summary>
        public string StrExt10 { get; set; }
        /// <summary>
        /// 扩展字符串字段11
        /// </summary>
        public string StrExt11 { get; set; }
        /// <summary>
        /// 扩展字符串字段12
        /// </summary>
        public string StrExt12 { get; set; }
        /// <summary>
        /// 扩展字符串字段13
        /// </summary>
        public string StrExt13 { get; set; }
        /// <summary>
        /// 扩展字符串字段14
        /// </summary>
        public string StrExt14 { get; set; }
        /// <summary>
        /// 扩展字符串字段15
        /// </summary>
        public string StrExt15 { get; set; }
        /// <summary>
        /// 扩展字符串字段16
        /// </summary>
        public string StrExt16 { get; set; }
        /// <summary>
        /// 扩展字符串字段17
        /// </summary>
        public string StrExt17 { get; set; }
        /// <summary>
        /// 扩展字符串字段18
        /// </summary>
        public string StrExt18 { get; set; }
        /// <summary>
        /// 扩展字符串字段19
        /// </summary>
        public string StrExt19 { get; set; }
        /// <summary>
        /// 扩展字符串字段20
        /// </summary>
        public string StrExt20 { get; set; }
        /// <summary>
        /// 扩展字符串字段21
        /// </summary>
        public string StrExt21 { get; set; }
        /// <summary>
        /// 扩展字符串字段22
        /// </summary>
        public string StrExt22 { get; set; }
        /// <summary>
        /// 扩展字符串字段23
        /// </summary>
        public string StrExt23 { get; set; }
        /// <summary>
        /// 扩展字符串字段24
        /// </summary>
        public string StrExt24 { get; set; }
        /// <summary>
        /// 扩展字符串字段25
        /// </summary>
        public string StrExt25 { get; set; }
        /// <summary>
        /// 扩展字符串字段26
        /// </summary>
        public string StrExt26 { get; set; }
        /// <summary>
        /// 扩展字符串字段27
        /// </summary>
        public string StrExt27 { get; set; }
        /// <summary>
        /// 扩展字符串字段28
        /// </summary>
        public string StrExt28 { get; set; }
        /// <summary>
        /// 扩展字符串字段29
        /// </summary>
        public string StrExt29 { get; set; }
        /// <summary>
        /// 扩展字符串字段30
        /// </summary>
        public string StrExt30 { get; set; }
        /// <summary>
        /// 扩展数字字段1
        /// </summary>
        public double? NumExt1 { get; set; }
        /// <summary>
        /// 扩展数字字段2
        /// </summary>
        public double? NumExt2 { get; set; }
        /// <summary>
        /// 扩展数字字段3
        /// </summary>
        public double? NumExt3 { get; set; }
        /// <summary>
        /// 扩展数字字段4
        /// </summary>
        public double? NumExt4 { get; set; }
        /// <summary>
        /// 扩展数字字段5
        /// </summary>
        public double? NumExt5 { get; set; }
        /// <summary>
        /// 扩展数字字段6
        /// </summary>
        public double? NumExt6 { get; set; }
        /// <summary>
        /// 扩展数字字段7
        /// </summary>
        public double? NumExt7 { get; set; }
        /// <summary>
        /// 扩展数字字段8
        /// </summary>
        public double? NumExt8 { get; set; }
        /// <summary>
        /// 扩展数字字段9
        /// </summary>
        public double? NumExt9 { get; set; }
        /// <summary>
        /// 扩展数字字段10
        /// </summary>
        public double? NumExt10 { get; set; }

        /// <summary>
        /// 扩展的关联对象
        /// </summary>
        [DataIgnore]
        [JsonConverter(typeof(OnlySeriaize))]
        public Dictionary<string, object> ExtObjects { get; set; }
    }
}
