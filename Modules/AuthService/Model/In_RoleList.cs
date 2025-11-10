using Common.Share;
using System;

namespace AuthService
{
    public class In_RoleList : BaseQueryParam
    {
        /// <summary>
        /// 角色名称
        /// </summary>
        public string roleName { get; set; }
        /// <summary>
        /// 角色状态（0正常 1停用）
        /// </summary>
        public string status { get; set; }
    }
}
