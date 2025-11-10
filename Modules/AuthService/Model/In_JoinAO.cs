using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Model
{
    public class In_JoinAO
    {
        /// <summary>
        /// 邀请码
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 要加入的职位
        /// </summary>
        public string postName { get; set; }
        /// <summary>
        /// 要加入的部门
        /// </summary>
        public long? deptId { get; set; }
    }
}
