using AuthService;
using AuthService.Model;
using FlowService.Model;
using IoTService.Models;
using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AfterService.Model
{
    /// <summary>
    /// 计划任务
    /// </summary>
    [TableName("mz_plane_task")]
    public class MZ_PlaneTask
    {
        [ID(false)]
        public string Id { get; set; }
        /// <summary>
        /// 所属组织ID
        /// </summary>
        public long? OrgId { get; set; }
        /// <summary>
        /// 任务单号：流程编号
        /// </summary>
        public string PlaneNumber { get; set; }
        /// <summary>
        /// 计划名称
        /// </summary>
        public string PlanName { get; set; }
        /// <summary>
        /// 设备计划Id
        /// </summary>
        public string PlanTypeId { get; set; }
        /// <summary>
        /// 关联流程任务Id
        /// </summary>
        public long? FlowId { get; set; }
        /// <summary>
        /// 关联的流程
        /// </summary>
        [DataIgnore]
        public V_FlowInfo FlowInfo { get; set; }
        /// <summary>
        /// 关联的待办列表
        /// </summary>
        [DataIgnore]
        public List<Out_FlowTodoItem> TodoTasks { get; set; }
        /// <summary>
        /// 状态：0进行中、1待执行、2执行中、3已完成、4已过期、5已验收、6验收失败、7已作废
        /// </summary>
        public byte? TaskStatus { get; set; }
        /// <summary>
        /// 设备Id
        /// </summary>
        public string TargetId { get; set; }
        /// <summary>
        /// 超时已提醒次数
        /// </summary>
        public int? NoticeCount { get; set; }
        /// <summary>
        /// 任务发起人所在部门
        /// </summary>
        public long? DeptId { get; set; }
        /// <summary>
        /// 任务的发起人
        /// </summary>
        public long? UserId { get; set; }
        /// <summary>
        /// 任务的发起人信息
        /// </summary>
        [DataIgnore]
        public MZ_AdminInfo UserInfo { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreatedOn { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartOn { get; set; }
        /// <summary>
        /// 截止时间
        /// </summary>
        public DateTime? EndOn { get; set; }
        /// <summary>
        /// 派工时间
        /// </summary>
        public DateTime? DispatchOn { get; set; }
        /// <summary>
        /// 派工人
        /// </summary>
        public long? DispatchUserId { get; set; }
        /// <summary>
        /// 执行时间
        /// </summary>
        public DateTime? ExecutedOn { get; set; }
        /// <summary>
        /// 执行人
        /// </summary>
        public long? ExeUserId { get; set; }
        /// <summary>
        /// 完成时间
        /// </summary>
        public DateTime? FinishedOn { get; set; }
        /// <summary>
        /// 验收时间
        /// </summary>
        public DateTime? CheckOn { get; set; }
        /// <summary>
        /// 验收人
        /// </summary>
        public long? CheckUserId { get; set; }
        /// <summary>
        /// 设备信息
        /// </summary>
        [DataIgnore]
        public MZ_IotDevice TargetDevice { get; set; }
        /// <summary>
        /// 设备所属房间
        /// </summary>
        [DataIgnore]
        public List<string> RoomNames { get; set; }
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
        /// 流程数据
        /// </summary>
        [DataIgnore]
        public Dictionary<string, string> DataItems { get; set; }
    }
}
