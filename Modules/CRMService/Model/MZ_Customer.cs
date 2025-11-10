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
    /// 客户表
    /// </summary>
    [TableName("mz_customer")]
    public class MZ_Customer : BaseEntity
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
        /// 所属部门
        /// </summary>
        public long? DeptId { get; set; }
        /// <summary>
        /// 客户唯一编号
        /// </summary>
        public string CustomerNumber { get; set; }
        /// <summary>
        /// 客户名称（新增必填）
        /// </summary>
        [RequiredWith(WithNullId = "Id", ErrorMessage = "请输入客户名称")]
        [MinLength(2), MaxLength(50)]
        public string CustomerName { get; set; }
        /// <summary>
        /// 客户类型：0为代理，1为直销（新增必填）
        /// </summary>
        [RequiredWith(WithNullId = "Id", ErrorMessage = "请选择客户类型")]
        public int? CustomerType { get; set; }
        /// <summary>
        /// 线索来源：微信线索weixin,流程表单form,其它other
        /// </summary>
        public string FromType { get; set; }
        /// <summary>
        /// 来源表单Id
        /// </summary>
        public string FromId { get; set; }
        /// <summary>
        /// 负责人
        /// </summary>
        public long? LeaderId { get; set; }
        /// <summary>
        /// 客户对应的企业Id,未绑定则为0
        /// </summary>
        public long? BindOrgId { get; set; }
        /// <summary>
        /// 经度
        /// </summary>
        public double? Lng { get; set; }
        /// <summary>
        /// 纬度
        /// </summary>
        public double? Lat { get; set; }
        /// <summary>
        /// 经纬度的geo编码
        /// </summary>
        public string Geo { get; set; }
        /// <summary>
        /// 省市区代码
        /// </summary>
        public string AddressCode { get; set; }
        /// <summary>
        /// 地址名称
        /// </summary>
        public string AddressName { get; set; }
        /// <summary>
        /// 详细地址
        /// </summary>
        public string AddressDetail { get; set; }
        /// <summary>
        /// 行业类型
        /// </summary>
        public int? Industry { get; set; }
        /// <summary>
        /// 公司电话
        /// </summary>
        public string CompanyTel { get; set; }
        /// <summary>
        /// 公司网址
        /// </summary>
        public string CompanyUrl { get; set; }
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
        /// 退回原因
        /// </summary>
        public string ReturnReason { get; set; }
        /// <summary>
        /// 删除标志（0代表存在、 2代表删除）
        /// </summary>
        public string del_flag { get; set; }
        /// <summary>
        /// 负责人姓名
        /// </summary>
        [DataIgnore]
        public string LeaderName { get; set; }
        /// <summary>
        /// 协作人员姓名
        /// </summary>
        [DataIgnore]
        public string HelperName { get; set; }
        /// <summary>
        /// 协作人信息
        /// </summary>
        public List<MZ_AdminInfo> HelperUsers { get; set; }
    }
}
