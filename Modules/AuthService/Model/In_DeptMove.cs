using System;
using System.Collections.Generic;

namespace AuthService.Model
{
    /// <summary>
    /// 部门移动参数
    /// </summary>
    public class In_DeptMove
    {
        /// <summary>
        /// 要移入的部门
        /// </summary>
        public List<long> ids { get; set; }
        /// <summary>
        /// 要移入的用户
        /// </summary>
        public List<long> uids { get; set; }
        /// <summary>
        /// 要移入的用户所属部门
        /// </summary>
        public long memDeptId { get; set; }
        /// <summary>
        /// 目标部门
        /// </summary>
        public long parentId { get; set; }
    }
}
