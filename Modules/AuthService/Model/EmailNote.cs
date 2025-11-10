using System;

namespace AuthService.Model
{
    /// <summary>
    /// 邮箱验证存储实体
    /// </summary>
    public class EmailNote
    {
        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long UserId { get; set; }
        /// <summary>
        /// 验证码
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 错误次数
        /// </summary>
        public int Num { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdatedOn { get; set; }
    }
}
