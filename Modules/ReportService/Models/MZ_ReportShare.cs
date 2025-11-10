using AuthService;
using Common.Share;
using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ReportService.Models
{
    [TableName("mz_report_share")]
    public class MZ_ReportShare : BaseEntity
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
        /// 报表Id
        /// </summary>
        public string ReportId { get; set; }
        /// <summary>
        /// 报表名称
        /// </summary>
        [DataIgnore]
        public string ReportName { get; set; }
        /// <summary>
        /// 报表类型：screen、table
        /// </summary>
        [DataIgnore]
        public string ReportType { get; set; }
        /// <summary>
        /// 报表的使用密码
        /// </summary>
        public string UsingPassword { get; set; }
        /// <summary>
        /// 分享的登录令牌
        /// </summary>
        public string TokenStr { get; set; }
        /// <summary>
        /// 有效时间(单位：天):0为永久
        /// </summary>
        [Required(ErrorMessage = "有效期不能为空")]
        public int? EffectiveTime { get; set; }
        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime? ExpirationTime { get; set; }


        /// <summary>
        /// Cron表达式
        /// </summary>
        public string TimerCron { get; set; }
        /// <summary>
        /// 定时器关联的Job
        /// </summary>
        public long? TimerJobId { get; set; }
        /// <summary>
        /// 通知对象类型：0为人员，1为角色
        /// </summary>
        public int? NoticeUserType { get; set; }
        /// <summary>
        /// 通知人员
        /// </summary>
        public string NoticeUsers { get; set; }
        /// <summary>
        /// 通知方式：APP站内通知,EMAIL邮件通知,SMS短信通知,WX微信通知，多个逗号隔开
        /// </summary>
        public string NoticeWay { get; set; }
        /// <summary>
        /// 是否启用定时推送（0启用 1停用）
        /// </summary>
        public string TimerStatus { get; set; }
        /// <summary>
        /// Cron的文字表述
        /// </summary>
        [DataIgnore]
        public string CronName { get; set; }
        /// <summary>
        /// 通知的用户列表
        /// </summary>
        [DataIgnore]
        public List<MZ_AdminInfo> NoticeUserList { get; set; }
        /// <summary>
        /// 通知的角色列表
        /// </summary>
        [DataIgnore]
        public List<MZ_Role> NoticeRoleList { get; set; }
    }
}
