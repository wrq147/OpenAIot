using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowService.FlowNode
{
    /// <summary>
    /// 办理人节点
    /// </summary>
    public class OperatorNode : FlowBaseNode
    {
        public BNProps props { get; set; }
    }
    /// <summary>
    /// 办理人节点的属性
    /// </summary>
    public class BNProps
    {
        /// <summary>
        /// 办理人选项类型
        /// </summary>
        public string assignedType { get; set; }
        /// <summary>
        /// 目标对象 人员/部门
        /// </summary>
        public ObjData[] assignedUser { get; set; }
        /// <summary>
        /// 角色
        /// </summary>
        public ObjData[] role { get; set; }
        /// <summary>
        /// 表单内联系人
        /// </summary>
        public string formUser { get; set; }
        /// <summary>
        /// 表单内设备
        /// </summary>
        public string formDevice { get; set; }
        /// <summary>
        /// 是否可变更办理人
        /// </summary>
        public bool enableChange { get; set; }
        /// <summary>
        /// 执行文本
        /// </summary>
        public string exeTxt { get; set; }
        /// <summary>
        /// 驳回文本
        /// </summary>
        public string refuseTxt { get; set; }
        /// <summary>
        /// 选项初始化
        /// </summary>
        public OptionInitItem[] optionInit { get; set; }
        /// <summary>
        /// 办理时限
        /// </summary>
        public OperatorTimeLimit timeLimit { get; set; }
        /// <summary>
        /// 表单权限
        /// </summary>
        public FormPermsItem[] formPerms { get; set; }

        /// <summary>
        /// 驳回设置
        /// </summary>
        public OperatorRefuseRes refuse { get; set; }

    }

    /// <summary>
    /// 时间限制
    /// </summary>
    public class OperatorTimeLimit
    {
        /// <summary>
        /// 超时时间设置
        /// </summary>
        public OperatorTimeout timeout { get; set; }
        /// <summary>
        /// 超时处理设置
        /// </summary>
        public OperatorHandler handler { get; set; }
    }
    public class OperatorTimeout
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
    public class OperatorHandler
    {
        /// <summary>
        /// 处理方式：REFUSE自动驳回，NOTIFY发送提醒
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 提醒设置
        /// </summary>
        public OperatorHandlerNotify notify { get; set; }
    }
    public class OperatorHandlerNotify
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
    public class OperatorRefuseRes
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

}
