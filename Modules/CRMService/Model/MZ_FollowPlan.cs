using AuthService;
using Common.Attr;
using Common.Share;
using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;

namespace CRMService.Model
{
    /// <summary>
    /// 跟进计划表
    /// </summary>
    [TableName("mz_follow_plan")]
    public class MZ_FollowPlan : BaseEntity
    {
        /// <summary>
        /// 编码
        /// </summary>
        [ID(false)]
        public string Id { get; set; }
        /// <summary>
        /// 所属企业Id
        /// </summary>
        public long? OrgId { get; set; }
        /// <summary>
        /// 所属部门（多个,号分隔）
        /// </summary>
        public string DeptIds { get; set; }
        /// <summary>
        /// 跟进的目标客户（必填）
        /// </summary>
        [RequiredWith(WithNullId = "Id", ErrorMessage = "请选择跟进的客户")]
        public string CustomerId { get; set; }
        /// <summary>
        /// 计划执行人（多个,号分隔）（必填）
        /// </summary>
        [RequiredWith(WithNullId = "Id", ErrorMessage = "请选择计划执行人")]
        public string Executor { get; set; }
        /// <summary>
        /// 计划执行人列表
        /// </summary>
        [DataIgnore]
        public List<MZ_AdminInfo> ExecutorUsers { get; set; }
        /// <summary>
        /// 计划时间（必填）
        /// </summary>
        [RequiredWith(WithNullId = "Id", ErrorMessage = "请选择计划时间")]
        public DateTime? PlanTime { get; set; }
        /// <summary>
        /// 计划内容（必填）
        /// </summary>
        [RequiredWith(WithNullId = "Id", ErrorMessage = "请输入计划内容")]
        public string Remark { get; set; }
        /// <summary>
        /// 状态：A待完成、F已完成
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 客户名称
        /// </summary>
        [DataIgnore]
        public string CustomerName { get; set; }
        /// <summary>
        /// 客户类型：0为代理，1为直销
        /// </summary>
        [DataIgnore]
        public int? CustomerType { get; set; }
    }
}
