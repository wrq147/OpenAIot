using System;

namespace AuthService.Model
{
    /// <summary>
    /// 短信验证存储实体
    /// </summary>
    public class SmsNote
    {
        /// <summary>
        /// 手机号
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 短信验证码
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
