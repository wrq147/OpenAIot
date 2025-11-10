using Common.Share;
using FlowService.Model;
using MyAccess.DB.Attr;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AfterService.Model
{
    [TableName("mz_flow")]
    public class V_FlowInfo : BaseEntity
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
        /// 流程实例名称
        /// </summary>
        public string FlowName { get; set; }
        /// <summary>
        /// 流程描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 完成或取消时间
        /// </summary>
        public DateTime? FinishTime { get; set; }
        /// <summary>
        /// 流程状态
        /// </summary>
        public FlowStatus Status { get; set; }
        public long? TemplateId { get; set; }
    }
}
