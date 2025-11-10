using Common.Share;
using MyAccess.DB.Attr;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FlowService.Model
{
    /// <summary>
    /// 流程实例
    /// </summary>
    [TableName("mz_flow")]
    public class MZ_Flow : BaseEntity
    {
        [ID(false)]
        public long? Id { get; set; }
        /// <summary>
        /// 流程第三方编号或关联工单
        /// </summary>
        public string FlowNumber { get; set; }
        /// <summary>
        /// 关联组织ID
        /// </summary>
        public long? OrgId { get; set; }
        /// <summary>
        /// 所属分组
        /// </summary>
        public long? GroupId { get; set; }
        /// <summary>
        /// 分组名
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [DataIgnore]
        public string GroupName { get; set; }
        /// <summary>
        /// 流程实例名称
        /// </summary>
        public string FlowName { get; set; }
        /// <summary>
        /// 流程描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 流程实例持久化
        /// </summary>
        public string PersistenceData { get; set; }
        /// <summary>
        /// 流程表单持久化
        /// </summary>
        public string FormFields { get; set; }
        /// <summary>
        /// 保存自选审核人节点数据
        /// </summary>
        public string Assign { get; set; }
        /// <summary>
        /// 完成或取消时间
        /// </summary>
        public DateTime? FinishTime { get; set; }
        /// <summary>
        /// 流程状态
        /// </summary>
        public FlowStatus Status { get; set; }
        /// <summary>
        /// 消息通知方式(json内容)
        /// </summary>
        public string notify { get; set; }
        [DataIgnore]
        [JsonIgnore]
        public FlowTemplateNotice notifyModel { get; set; }
        public long? TemplateId { get; set; }
        /// <summary>
        /// 删除标志（0代表存在 2代表删除）
        /// </summary>
        public string del_flag { get; set; }
        /// <summary>
        /// 是否为嵌入式流程
        /// </summary>
        public bool? IsEmbed { get; set; }
        /// <summary>
        /// 关联的执行节点
        /// </summary>
        [DataIgnore]
        public List<MZ_Flow_Node> ExecutionNodes { get; set; }

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
    }

    public enum FlowStatus
    {
        /// <summary>
        /// 运行
        /// </summary>
        Runnable = 0,
        /// <summary>
        /// 保存中
        /// </summary>
        Suspended = 1,
        /// <summary>
        /// 完成
        /// </summary>
        Complete = 2,
        /// <summary>
        /// 取消
        /// </summary>
        Terminated = 3
    }
}
