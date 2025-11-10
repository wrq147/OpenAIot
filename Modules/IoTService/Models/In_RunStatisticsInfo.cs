using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTService.Models
{
    public class In_RunStatisticsInfo
    {
        /// <summary>
        /// 客户组织Id
        /// </summary>
        public long OrgId { get; set; }
        /// <summary>
        /// 状态列表
        /// </summary>
        public string[] StateList { get; set; }
    }
}
