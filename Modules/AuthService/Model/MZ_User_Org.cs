using MyAccess.DB.Attr;
using System;

namespace AuthService.Model
{
    /// <summary>
    /// 用户组织关联表
    /// </summary>
    [TableName("mz_user_org")]
    public class MZ_User_Org
    {
        /// <summary>
        /// 关联用户ID
        /// </summary>
        [ID(false)]
        public long? UserId { get; set; }
        /// <summary>
        /// 关联组织ID
        /// </summary>
        [ID(false)]
        public long? OrgId { get; set; }
        /// <summary>
        /// 所属部门
        /// </summary>
        [ID(false)]
        public long? dept_id { get; set; }
        /// <summary>
        /// 所属部门名称
        /// </summary>
        [DataIgnore]
        public string dept_name { get; set; }
        /// <summary>
        /// 当前职位
        /// </summary>
        public string post_name { get; set; }
        /// <summary>
        /// 是否为部门领导
        /// </summary>
        public bool? IsLeader { get; set; }
        /// <summary>
        /// 是否为主要部门
        /// </summary>
        public bool? IsPrimary { get; set; }
    }
}
