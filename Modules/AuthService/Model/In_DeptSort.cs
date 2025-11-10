using System;
using System.Collections.Generic;

namespace AuthService.Model
{
    public class In_DeptSort
    {
        /// <summary>
        /// 要排序的部门，按排序放在列表里
        /// </summary>
        public List<long> idList { get; set; }
        public long orgId { get; set; }
    }
}
