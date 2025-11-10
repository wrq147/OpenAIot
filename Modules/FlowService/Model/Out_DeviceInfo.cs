using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowService.Model
{
    public class Out_DeviceInfo
    {
        public string Id { get; set; }
        /// <summary>
        /// 来源组织ID
        /// </summary>
        public long? OrgId { get; set; }
        /// <summary>
        /// 拥有者组织ID
        /// </summary>
        public long? OwnerOrgId { get; set; }
        /// <summary>
        /// 使用者组织ID（使用者为组织）
        /// </summary>
        public long? UseOrgId { get; set; }
        /// <summary>
        /// 当前使用者（使用者为个人）
        /// </summary>
        public long? UseUserId { get; set; }
    }
}
