using Common.Attr;
using Common.Share;
using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;

namespace IoTRulesService.Model
{
    [TableName("mz_rule_template")]
    public class MZ_RuleTemplate : BaseEntity
    {
        [ID(false)]
        public long? Id { get; set; }
        /// <summary>
        /// 规则模板关联组织ID
        /// </summary>
        public long? OrgId { get; set; }
        /// <summary>
        /// 分组Id
        /// </summary>
        public string GroupId { get; set; }
        /// <summary>
        /// 规则名称
        /// </summary>
        [RequiredWith(WithNullId = "Id", ErrorMessage = "请输入规则名称")]
        public string Name { get; set; }
        /// <summary>
        /// 创建源：pc、mobile
        /// </summary>
        public string CreatedFrom { get; set; }
        /// <summary>
        /// 触发方式：0为设备，1为Http，2为定时
        /// </summary>
        [RequiredWith(WithNullId = "Id", ErrorMessage = "请选择触发方式")]
        public byte? TriggerWay { get; set; }
        /// <summary>
        /// Cron表达式（定时触发用）
        /// </summary>
        public string TimerCron { get; set; }
        /// <summary>
        /// Cron的文字表述
        /// </summary>
        [DataIgnore]
        public string CronName { get; set; }
        /// <summary>
        /// 定时器关联的Job
        /// </summary>
        public long? TimerJobId { get; set; }
        /// <summary>
        /// 参数json字符串
        /// </summary>
        public string HttpParams { get; set; }
        /// <summary>
        /// 规则优先级(越小越前）
        /// </summary>
        public int? Sort { get; set; }
        /// <summary>
        /// 状态（0正常 1暂停）
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 规则流程内容
        /// </summary>
        public string RuleJson { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 规则的触发方式列表
        /// </summary>
        [DataIgnore]
        public List<MZ_RuleTrigger> TriggerList { get; set; }
        /// <summary>
        /// 规则是否存在可修改参数
        /// </summary>
        [DataIgnore]
        public bool EnableParam { get; set; }
    }
}
