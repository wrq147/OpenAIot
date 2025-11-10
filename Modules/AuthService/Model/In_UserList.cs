using Common.Share;
using System;

namespace AuthService
{
    public class In_UserList : BaseQueryParam
    {
        public string userName { get; set; }
        public string phonenumber { get; set; }
        public string status { get; set; }
        public long? deptId { get; set; }
        /// <summary>
        /// 过滤部门Id列表
        /// </summary>
        public long[] deptIdList { get; set; }
        /// <summary>
        /// 判断deptId过滤是否包含子部门，默认true包含
        /// </summary>
        public bool? deptIdWithChildren { get; set; }
        public string depAncestors { get; set; }
        public long? userId { get; set; }
        public long? orgId { get; set; }
        /// <summary>
        /// 过滤用户名、手机号、姓名
        /// </summary>
        public string key { get; set; }
        /// <summary>
        /// 过滤主要部门标记
        /// </summary>
        public bool? isPrimaryDept { get; set; }
        /// <summary>
        /// 过滤我为负责人的部门成员
        /// </summary>
        public bool? isMyLeader { get; set; }
    }
}
