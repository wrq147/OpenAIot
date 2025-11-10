using AuthService;
using Common.Attr;
using Common.Share;
using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CRMService.Model
{
    /// <summary>
    /// 商机表
    /// </summary>
    [TableName("mz_opportunity")]
    public class MZ_Opportunity : BaseEntity
    {
        /// <summary>
        /// 编码
        /// </summary>
        [ID(false)]
        public string Id { get; set; }
        /// <summary>
        /// 商机唯一编号
        /// </summary>
        public string OpportNumber { get; set; }
        /// <summary>
        /// 所属企业Id
        /// </summary>
        public long? OrgId { get; set; }
        /// <summary>
        /// 所属部门
        /// </summary>
        public long? DeptId { get; set; }
        /// <summary>
        /// 负责人（新增必填）
        /// </summary>
        [RequiredWith(WithNullId = "Id", ErrorMessage = "请选择负责人")]
        public long? LeaderId { get; set; }
        /// <summary>
        /// 商机名称（新增必填）
        /// </summary>
        [RequiredWith(WithNullId = "Id", ErrorMessage = "请输入商机名称")]
        [MinLength(2), MaxLength(50)]
        public string OpportName { get; set; }
        /// <summary>
        /// 客户Id（新增必填）
        /// </summary>
        [RequiredWith(WithNullId = "Id", ErrorMessage = "请选择商机")]
        public string CustomerId { get; set; }
        /// <summary>
        /// 联系人Id（新增必填）
        /// </summary>
        [RequiredWith(WithNullId = "Id", ErrorMessage = "请选择联系人")]
        public string ContactId { get; set; }
        /// <summary>
        /// 预计成交几率，单位%（新增必填）
        /// </summary>
        [RequiredWith(WithNullId = "Id", ErrorMessage = "请输入赢率")]
        public float? Probability { get; set; }
        /// <summary>
        /// 销售阶段（新增必填）
        /// </summary>
        [RequiredWith(WithNullId = "Id", ErrorMessage = "请选择销售阶段")]
        public string Period { get; set; }
        /// <summary>
        /// 阶段类型（不用传）
        /// </summary>
        public string PeriodType { get; set; }
        /// <summary>
        /// 协作者（多个,号分隔）
        /// </summary>
        public string Helper { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 输单原因
        /// </summary>
        public string LoseRemark { get; set; }
        /// <summary>
        /// 最后一条跟进
        /// </summary>
        public string LastFollowId { get; set; }
        /// <summary>
        /// 最后一条跟进时间
        /// </summary>
        public DateTime? LastFollowDate { get; set; }
        /// <summary>
        /// 领取时间
        /// </summary>
        public DateTime? StartFollowDate { get; set; }
        /// <summary>
        /// 退回原因
        /// </summary>
        public string ReturnReason { get; set; }
        /// <summary>
        /// 删除标志（0代表存在 2代表删除）
        /// </summary>
        public string del_flag { get; set; }
        /// <summary>
        /// 明细表
        /// </summary>
        [DataIgnore]
        public List<MZ_OpportDetail> DetailList { get; set; }

        /// <summary>
        /// 负责人信息
        /// </summary>
        [DataIgnore]
        public MZ_AdminInfo LeaderUser { get; set; }
        /// <summary>
        /// 联系人信息
        /// </summary>
        [DataIgnore]
        public MZ_Contact ContactUser { get; set; }
        /// <summary>
        /// 客户名称
        /// </summary>
        [DataIgnore]
        public string CustomerName { get; set; }
        /// <summary>
        /// 客户编号
        /// </summary>
        [DataIgnore]
        public string CustomerNumber { get; set; }
        /// <summary>
        /// 销售阶段
        /// </summary>
        [DataIgnore]
        public string PeriodName { get; set; }
        /// <summary>
        /// 协作人信息
        /// </summary>
        public List<MZ_AdminInfo> HelperUsers { get; set; }
    }
}
