using AuthService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Model
{
    public class Out_UserDept : MZ_Dept
    {
        /// <summary>
        /// 当前职位
        /// </summary>
        public string post_name { get; set; }
        /// <summary>
        /// 是否为部门领导
        /// </summary>
        public bool? IsLeader { get; set; }
        /// <summary>
        /// 是否为主要部门
        /// </summary>
        public bool? IsPrimary { get; set; }
    }
}
