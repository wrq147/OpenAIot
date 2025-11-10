

using MyAccess.DB.Attr;

namespace AuthService
{
    [TableName("mz_role_permission")]
    public class MZ_Role_Permission
    {
        public long RoleID { get; set; }
        public long MenuId { get; set; }
    }
}
