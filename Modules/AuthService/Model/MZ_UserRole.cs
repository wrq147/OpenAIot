
using MyAccess.DB.Attr;
using System;
namespace AuthService
{
    [TableName("mz_user_role")]
    public class MZ_UserRole
    {
        [ID(false)]
        public long? UserId { get; set; }
        [ID(false)]
        public long? RoleID { get; set; }
        [ID(false)]
        public long? OrgId { get; set; }
    }
}
