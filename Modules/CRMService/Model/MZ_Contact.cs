using AuthService;
using Common.Attr;
using Common.Share;
using FluentMigrator.Infrastructure;
using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMService.Model
{
    /// <summary>
    /// 联系人
    /// </summary>
    [TableName("mz_contact")]
    public class MZ_Contact : BaseEntity
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
        /// 姓名（新增必填）
        /// </summary>
        [RequiredWith(WithNullId = "Id", ErrorMessage = "请输入姓名")]
        [MinLength(1), MaxLength(50)]
        public string RealName { get; set; }
        /// <summary>
        /// 手机号码（新增必填）
        /// </summary>
        [RequiredWith(WithNullId = "Id", ErrorMessage = "请输入手机号码")]
        [RegularExpression(pattern: @"^(13|14|15|16|17|18|19)[0-9]{9}$", ErrorMessage = "手机号码格式错误")]
        public string Mobile { get; set; }
        /// <summary>
        /// 职务
        /// </summary>
        public string PostName { get; set; }
        /// <summary>
        /// 部门
        /// </summary>
        public string DeptName { get; set; }
        /// <summary>
        /// 性别（0男 1女 2未知）
        /// </summary>
        public string Sex { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 微信号
        /// </summary>
        public string WxNumber { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 负责人（新增必填）
        /// </summary>
        [RequiredWith(WithNullId = "Id", ErrorMessage = "请选择负责人")]
        public long? LeaderId { get; set; }
        /// <summary>
        /// 所属部门
        /// </summary>
        public long? DeptId { get; set; }
        /// <summary>
        /// 协作者（多个,号分隔）
        /// </summary>
        public string Helper { get; set; }
        /// <summary>
        /// 客户Id（新增必填）
        /// </summary>
        [RequiredWith(WithNullId = "Id", ErrorMessage = "请选择客户")]
        public string CustomerId { get; set; }
        /// <summary>
        /// 删除标志（0代表存在、 2代表删除）
        /// </summary>
        public string del_flag { get; set; }

        /// <summary>
        /// 协作人员列表
        /// </summary>
        [DataIgnore]
        public List<MZ_AdminInfo> HelperUsers { get; set; }
        /// <summary>
        /// 负责人信息
        /// </summary>
        [DataIgnore]
        public MZ_AdminInfo LeaderUser { get; set; }
        /// <summary>
        /// 客户名称
        /// </summary>
        [DataIgnore]
        public string CustomerName { get; set; }
    }
}
