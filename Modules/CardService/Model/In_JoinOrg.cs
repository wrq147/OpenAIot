using System;
namespace CardService.Model
{
    /// <summary>
    /// 邀请加入的参数
    /// </summary>
    public class In_JoinOrg
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
        /// 要加入的真实姓名
        /// </summary>
        public string realName { get; set; }
        /// <summary>
        /// 要加入的部门
        /// </summary>
        public long? deptId { get; set; }
    }
}
