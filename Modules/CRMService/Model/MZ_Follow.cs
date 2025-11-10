using AuthService;
using Common.Attr;
using Common.Share;
using FluentMigrator.Infrastructure;
using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMService.Model
{
    /// <summary>
    /// 跟进记录表
    /// </summary>
    [TableName("mz_follow")]
    public class MZ_Follow : BaseEntity
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
        /// 跟进目标类型：0为客户、1为线索（新增必填）
        /// </summary>
        [RequiredWith(WithNullId = "Id", ErrorMessage = "请选择跟进主体类型")]
        public int? TargetType { get; set; }
        /// <summary>
        /// 跟进目标Id（新增必填）
        /// </summary>
        [RequiredWith(WithNullId = "Id", ErrorMessage = "请选择跟进主体")]
        public string TargetId { get; set; }
        /// <summary>
        /// 商机Id
        /// </summary>
        public string OpportId { get; set; }
        /// <summary>
        /// 跟进内容
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 跟进时间（新增必填）
        /// </summary>
        [RequiredWith(WithNullId = "Id", ErrorMessage = "请选择跟进时间")]
        public DateTime? FollowTime { get; set; }
        /// <summary>
        /// 跟进人（新增必填）
        /// </summary>
        [RequiredWith(WithNullId = "Id", ErrorMessage = "请选择跟进人")]
        public long? FollowUser { get; set; }
        /// <summary>
        /// 跟进人信息
        /// </summary>
        [DataIgnore]
        public MZ_AdminInfo FollowUserInfo{ get; set; }
        /// <summary>
        /// 联系人Id
        /// </summary>
        public string ContactId { get; set; }
        /// <summary>
        /// 联系人信息
        /// </summary>
        [DataIgnore]
        public MZ_Contact ContactInfo { get; set; }
        /// <summary>
        /// 跟进方式
        /// </summary>
        public string FollowWay { get; set; }
        /// <summary>
        /// 删除标志（0代表存在、 2代表删除）
        /// </summary>
        public string del_flag { get; set; }
        /// <summary>
        /// 跟进目标名称
        /// </summary>
        [DataIgnore]
        public string TargetName { get; set; }
        /// <summary>
        /// 商机名称
        /// </summary>
        [DataIgnore]
        public string OpportName { get; set; }
    }
}
