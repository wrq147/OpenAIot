using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Model
{
    [TableName("mz_role_scope")]
    public class MZ_RoleScope
    {
        /// <summary>
        /// 角色Id
        /// </summary>
        [ID(false)]
        public long? RoleID { get; set; }
        /// <summary>
        /// 菜单Id
        /// </summary>
        [ID(false)]
        public long? MenuId { get; set; }
        /// <summary>
        /// 数据范围（1：全部数据权限 2：自定数据权限 3：本部门数据权限 4：本部门及以下数据权限 5:仅本人数据权限）
        /// </summary>
        public string DataScope { get; set; }
        /// <summary>
        /// 自定义数据权限Json
        /// </summary>
        public string CustomScope { get; set; }
    }
}
