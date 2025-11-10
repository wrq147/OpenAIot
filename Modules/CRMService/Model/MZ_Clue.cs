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
    /// 线索表
    /// </summary>
    [TableName("mz_clue")]
    public class MZ_Clue : BaseEntity
    {
        /// <summary>
        /// 编码
        /// </summary>
        [ID(false)]
        public string Id { get; set; }
        /// <summary>
        /// 企业Id
        /// </summary>
        public long? OrgId { get; set; }
        /// <summary>
        /// 所属部门
        /// </summary>
        public long? DeptId { get; set; }
        /// <summary>
        /// 联系人（新增必填）
        /// </summary>
        [RequiredWith(WithNullId = "Id", ErrorMessage = "请输入联系人")]
        [MinLength(1), MaxLength(50)]
        public string RealName { get; set; }
        /// <summary>
        /// 手机号码（新增必填）
        /// </summary>
        [RequiredWith(WithNullId = "Id", ErrorMessage = "请输入手机号码")]
        [RegularExpression(pattern: @"^(13|14|15|16|17|18|19)[0-9]{9}$", ErrorMessage = "手机号码格式错误")]
        public string Mobile { get; set; }
        /// <summary>
        /// 客户名称（新增必填）
        /// </summary>
        [RequiredWith(WithNullId = "Id", ErrorMessage = "请输入客户名称")]
        [MinLength(2), MaxLength(50)]
        public string CompanyName { get; set; }
        /// <summary>
        /// 线索来源：微信线索weixin,流程表单form,其它other
        /// </summary>
        public string FromType { get; set; }
        /// <summary>
        /// 来源表单Id
        /// </summary>
        public string FromId { get; set; }
        /// <summary>
        /// 线索跟进人（为0则为公海线索）
        /// </summary>
        public long? LeaderId { get; set; }
        /// <summary>
        /// 职务
        /// </summary>
        public string PostName { get; set; }
        /// <summary>
        /// 部门
        /// </summary>
        public string DeptName { get; set; }
        /// <summary>
        /// 协作者（多个,号分隔）
        /// </summary>
        public string Helper { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
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
        /// 转换时间
        /// </summary>
        public DateTime? ChangeDate { get; set; }
        /// <summary>
        /// 转换的目标客户Id
        /// </summary>
        public string ChangeId { get; set; }
        /// <summary>
        /// 是否在客户中同步显示跟进
        /// </summary>
        public bool? SyncFollow { get; set; }
        /// <summary>
        /// 退回原因
        /// </summary>
        public string ReturnReason { get; set; }
        /// <summary>
        /// 删除标志（0代表存在、1代表被转换、 2代表删除）
        /// </summary>
        public string del_flag { get; set; }

        /// <summary>
        /// 负责人姓名
        /// </summary>
        [DataIgnore]
        public string LeaderName { get; set; }
        /// <summary>
        /// 协作人员名称
        /// </summary>
        [DataIgnore]
        public string HelperName { get; set; }

        /// <summary>
        /// 协作人信息
        /// </summary>
        public List<MZ_AdminInfo> HelperUsers { get; set; }
    }
}
