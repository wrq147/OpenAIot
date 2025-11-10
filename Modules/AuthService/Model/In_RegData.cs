using System;
using System.ComponentModel.DataAnnotations;

namespace AuthService.Model
{
    public class In_RegData
    {
        /// <summary>
        /// 员工邀请码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 头像（需要库Base64图片格式）
        /// </summary>
        public string Avatar { get; set; }
        /// <summary>
        /// 真实姓名
        /// </summary>
        public string RealName { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 部门Id
        /// </summary>
        public long? DeptId { get; set; }
        /// <summary>
        /// 职位
        /// </summary>
        public string PostName { get; set; }
    }
}
