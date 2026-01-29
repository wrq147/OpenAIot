using AuthService;
using Common.Share;
using IoTService.Models;
using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;

namespace AfterService.Model
{
    /// <summary>
    /// 计划类型
    /// </summary>
    [TableName("mz_plane_type")]
    public class MZ_PlaneType : BaseEntity
    {
        /// <summary>
        /// 编码
        /// </summary>
        [ID(false)]
        public string Id { get; set; }
        /// <summary>
        /// 计划名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 所属组织ID
        /// </summary>
        public long? OrgId { get; set; }
        /// <summary>
        /// 0为手动发起、1为定时发起、2为设备事件发起
        /// </summary>
        public int? StartWay { get; set; }
        /// <summary>
        /// Cron表达式(定时计划才需要填）
        /// </summary>
        public string TimerCron { get; set; }
        /// <summary>
        /// 定时执行描述
        /// </summary>
        [DataIgnore]
        public string TimerName { get; set; }
        /// <summary>
        /// 计划执行天数，为0不限制
        /// </summary>
        public int? PlaneDays { get; set; }
        /// <summary>
        /// 是否排除假期
        /// </summary>
        public bool? ExcludeHoliday { get; set; } = false;
        /// <summary>
        /// 是否限制展示
        /// </summary>
        public bool? IsFilterLeader { get; set; } = false;
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 派工流程模板Id
        /// </summary>
        public long? FlowTemplateId { get; set; }
        /// <summary>
        /// 图标
        /// </summary>
        [DataIgnore]
        public string Icon { get; set; }
        /// <summary>
        /// 背景色
        /// </summary>
        [DataIgnore]
        public string Background { get; set; }
        /// <summary>
        /// 流程模板名称
        /// </summary>
        [DataIgnore]
        public string FlowTemplateName { get; set; }
        /// <summary>
        /// 流程初始化json
        /// </summary>
        public string FlowInitJson { get; set; }
        /// <summary>
        /// 超期提醒Json:格式为[{'day':3,'way':1,ccway:0,'target':[{'userid':1,'img':'','name':''}]}]
        /// </summary>
        public string ExpireNotices { get; set; }
        /// <summary>
        /// 流程的发起人(定时计划才需要填）
        /// </summary>
        public long? FlowCreatedUserId { get; set; }
        /// <summary>
        /// 发起人
        /// </summary>
        [DataIgnore]
        public MZ_AdminInfo FlowCreatedUser { get; set; }
        /// <summary>
        /// 计划事件
        /// </summary>
        public List<MZ_PlaneEvent> Events { get; set; }
        /// <summary>
        /// 计划目标
        /// </summary>
        [DataIgnore]
        public List<MZ_PlaneTarget> Targets { get; set; }
    }
}
