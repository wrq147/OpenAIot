using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Model
{
    public class In_JoinUser
    {
        /// <summary>
        /// 邀请的用户Id
        /// </summary>
        public long uid { get; set; }
        /// <summary>
        /// 邀请加入的部门
        /// </summary>
        public long depId { get; set; }
        /// <summary>
        /// 邀请加入的职位
        /// </summary>
        public string postName { get; set; }
    }
}
