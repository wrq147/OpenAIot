using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowService.Model
{
    public class In_OrgList
    {
        /// <summary>
        /// 过滤组织
        /// </summary>
        public long? orgId { get; set; }
        /// <summary>
        /// 当前部门
        /// </summary>
        public long? deptId { get; set; }
        /// <summary>
        /// 获取组织数据：user=人和部门  dept=只能选部门
        /// </summary>
        public string type { get; set; }
    }
}
