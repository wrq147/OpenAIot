using System;

namespace AuthService.Model
{
    /// <summary>
    /// 角色权限
    /// </summary>
    public class Out_RolePermiss
    {
        public long RoleID { get; set; }
        public string perms { get; set; }
    }
}
