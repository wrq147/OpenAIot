using AuthService;
using Common.Attr;
using Common.Share;
using MyAccess.DB.Attr;
using System.Collections.Generic;

namespace ReportService.Models
{
    [TableName("mz_report_warn")]
    public class MZ_ReportWarn : BaseEntity
    {
        /// <summary>
        /// 编码Id
        /// </summary>
        [ID(false)]
        public string Id { get; set; }
        /// <summary>
        /// 所属组织ID
        /// </summary>
        public long? OrgId { get; set; }
        /// <summary>
        /// 所属报表Id
        /// </summary>
        public string ReportId { get; set; }
        /// <summary>
        /// 预警名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 条件json
        /// </summary>
        public string ConditionJson { get; set; }
        /// <summary>
        /// Cron表达式
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
        /// 沉默周期，单位秒（默认表示86400S，最小60秒）
        /// </summary>
        public int? SilenceTime { get; set; }
        /// <summary>
        /// 通知对象类型：0为人员，1为角色
        /// </summary>
        public int? NoticeUserType { get; set; }
        /// <summary>
        /// 通知人员
        /// </summary>
        public string NoticeUsers { get; set; }
        [DataIgnore]
        public List<MZ_AdminInfo> NoticeUserList { get; set; }
        /// <summary>
        /// 通知方式：APP站内通知,EMAIL邮件通知,SMS短信通知,WX微信通知，多个逗号隔开
        /// </summary>
        public string NoticeWay { get; set; }
        /// <summary>
        /// 状态（0启用 1停用）
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 报表分享Id
        /// </summary>
        public string ShareId { get; set; }
    }
}
