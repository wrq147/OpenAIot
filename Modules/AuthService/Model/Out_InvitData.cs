using Common.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Model
{
    public class Out_InvitData
    {
        public long OrgId { get; set; }
        public string OrgName { get; set; }
        public List<TreeSelect<long>> DeptList { get; set; }
    }
}
