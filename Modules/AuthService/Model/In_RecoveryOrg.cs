using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Model
{
    public class In_RecoveryOrg
    {
        /// <summary>
        /// 要恢复的组织Id
        /// </summary>
        public long OrgId { get; set; }
        /// <summary>
        /// 要恢复的用户Id
        /// </summary>
        public long UserId { get; set; }
        /// <summary>
        /// 恢复的角色Id列表
        /// </summary>
        public long[] Roles { get; set; }
    }
}
