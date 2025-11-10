using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMService.Model
{
    public class JoinEventData
    {
        public string BindCustomerId { get; set; }
        public long OrgId { get; set; }
        public long UserId { get; set; }
    }
}
