using System;

namespace CardService.Model
{
    /// <summary>
    /// 邀请参数
    /// </summary>
    public class In_InviteOrg
    {
        /// <summary>
        /// 邀请加入的组织
        /// </summary>
        public long? OrgId { get; set; }
        /// <summary>
        /// 邀请加入的部门
        /// </summary>
        public long? DeptId { get; set; }
    }
}
