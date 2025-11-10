using Common.Attr;
using FluentMigrator.Infrastructure;
using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageService.Model
{
    /// <summary>
    /// 仓库实体
    /// </summary>
    [TableName("mz_store_house")]
    public class MZ_StoreHouse
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
        /// 仓库名称
        /// </summary>
        [RequiredWith(WithNullId = "Id", ErrorMessage = "请输入仓库名称")]
        public string StoreName { get; set; }
        /// <summary>
        /// 是否为系统仓库：0为否，1为是，系统仓库不可删除、不可停用
        /// </summary>
        public int? IsSystem { get; set; }
        /// <summary>
        /// 状态：0为停用，1为正常
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 删除标志（0代表存在、 2代表删除）
        /// </summary>
        public string del_flag { get; set; }
        /// <summary>
        /// 负责人
        /// </summary>
        public long? LeaderId { get; set; }
        /// <summary>
        /// 所属部门
        /// </summary>
        public long? DeptId { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 出库审核模板
        /// </summary>
        public long? LeaveTemplateId { get; set; }
        /// <summary>
        /// 入库审核模板
        /// </summary>
        public long? EnterTemplateId { get; set; }
        /// <summary>
        /// 出库申请单审核模板
        /// </summary>
        public long? LeaveApplyTemplateId { get; set; }
        /// <summary>
        /// 出库流程初始化json
        /// </summary>
        public string LeaveFlowInitJson { get; set; }
        /// <summary>
        /// 入库流程初始化json
        /// </summary>
        public string EnterFlowInitJson { get; set; }
        /// <summary>
        /// 出库申请流程初始化json
        /// </summary>
        public string LeaveApplyFlowInitJson { get; set; }
        /// <summary>
        /// 负责人名称
        /// </summary>
        [DataIgnore]
        public string LeaderName { get; set; }
        /// <summary>
        /// 出库流程模板名称
        /// </summary>
        [DataIgnore]
        public string LeaveTemplateName { get; set; }
        /// <summary>
        /// 入库流程模板名称
        /// </summary>
        [DataIgnore]
        public string EnterTemplateName { get; set; }
        /// <summary>
        /// 出库申请单模板名称
        /// </summary>
        [DataIgnore]
        public string LeaveApplyTemplateName { get; set; }
    }
}
