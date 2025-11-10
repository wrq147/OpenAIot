using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProducerService.Model
{
    public class In_JoinInvite
    {
        /// <summary>
        /// 邀请Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 要加入的组织Id
        /// </summary>
        public long? OrgId { get; set; }
    }
}
