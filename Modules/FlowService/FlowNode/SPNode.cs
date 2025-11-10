using Common.IdGenerator;
using FlowService.DAL;
using FlowService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace FlowService.FlowNode
{
    /// <summary>
    /// 审批节点
    /// </summary>
    public class SPNode : FlowBaseNode
    {
        public UserProps props { get; set; }
    }


    /// <summary>
    /// 审批节点的属性
    /// </summary>
    public class UserProps
    {
        /// <summary>
        /// 审批人选项类型
        /// </summary>
        public string assignedType { get; set; }
        /// <summary>
        /// 审批模式 会签/或签
        /// </summary>
        public string mode { get; set; }
        /// <summary>
        /// 是否需要签字
        /// </summary>
        public bool sign { get; set; }
        /// <summary>
        /// 选项初始化
        /// </summary>
        public OptionInitItem[] optionInit { get; set; }
        /// <summary>
        /// 审批时限
        /// </summary>
        public ExamineTimeLimit timeLimit { get; set; }
        /// <summary>
        /// 目标对象 人员/部门
        /// </summary>
        public ObjData[] assignedUser { get; set; }
        /// <summary>
        /// 表单权限
        /// </summary>
        public FormPermsItem[] formPerms { get; set; }
        /// <summary>
        /// 自选
        /// </summary>
        public SelfSelect selfSelect { get; set; }
        /// <summary>
        /// 连续多级主管
        /// </summary>
        public LeaderTop leaderTop { get; set; }
        /// <summary>
        /// 主管
        /// </summary>
        public Leader leader { get; set; }
        /// <summary>
        /// 角色
        /// </summary>
        public ObjData[] role { get; set; }
        /// <summary>
        /// 驳回设置
        /// </summary>
        public RefuseRes refuse { get; set; }
        /// <summary>
        /// 表单内联系人
        /// </summary>
        public string formUser { get; set; }
        /// <summary>
        /// 未填审批人时的处理方式
        /// </summary>
        public Nobody nobody { get; set; }
    }
    public class Nobody
    {
        public string handler { get; set; }
        public ObjData[] assignedUser { get; set; }
    }
    public class RefuseRes
    {
        /// <summary>
        /// TO_END直接结束流程,TO_BEFORE驳回到上级审批节点,TO_NODE驳回到指定节点
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// TO_NODE时指定的节点id
        /// </summary>
        public string target { get; set; }
    }
    public class SelfSelect
    {
        /// <summary>
        /// true自选多个人,false自选一个人
        /// </summary>
        public bool multiple { get; set; }
    }
    public class LeaderTop
    {
        /// <summary>
        /// TOP直到最上层主管,LEAVE不超过发起人的
        /// </summary>
        public string endCondition { get; set; }
        /// <summary>
        /// LEAVE第几级主管
        /// </summary>
        public int endLevel { get; set; }
    }
    public class Leader
    {
        /// <summary>
        /// 第几级主管
        /// </summary>
        public int level { get; set; }
    }
    /// <summary>
    /// 时间限制
    /// </summary>
    public class ExamineTimeLimit
    {
        /// <summary>
        /// 超时时间设置
        /// </summary>
        public ExamineTimeout timeout { get; set; }
        /// <summary>
        /// 超时处理设置
        /// </summary>
        public ExamineHandler handler { get; set; }
    }
    public class ExamineTimeout
    {
        /// <summary>
        /// H表示小时、D表示天
        /// </summary>
        public string unit { get; set; }
        /// <summary>
        /// 时长
        /// </summary>
        public int value { get; set; }
    }
    public class ExamineHandler
    {
        /// <summary>
        /// 处理方式：PASS自动通过，REFUSE自动驳回，NOTIFY发送提醒
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 提醒设置
        /// </summary>
        public ExamineHandlerNotify notify { get; set; }
    }
    public class ExamineHandlerNotify
    {
        /// <summary>
        /// 是否只提醒一次
        /// </summary>
        public bool once { get; set; }
        /// <summary>
        /// 间隔几小时提醒
        /// </summary>
        public int hour { get; set; }
    }

}
