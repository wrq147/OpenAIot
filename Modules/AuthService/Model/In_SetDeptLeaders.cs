using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Model
{
    public class In_SetDeptLeaders
    {
        /// <summary>
        /// 设置的部门Id
        /// </summary>
        public long depId { get; set; }
        /// <summary>
        /// 设置的负责人列表
        /// </summary>
        public long[] Leaders { get; set; }
    }
}
