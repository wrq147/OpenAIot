using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Model
{
    public class In_UserRoleList : In_UserList
    {
        public long roleId { get; set; }
    }
}
