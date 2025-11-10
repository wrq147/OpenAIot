using System;
namespace ProducerService.Model
{
    public class In_YqLogin
    {
        /// <summary>
        /// 手机邀请验证码
        /// </summary>
        public string MobileCode { get; set; }
        /// <summary>
        /// 手机邀请码
        /// </summary>
        public string YqCode { get; set; }
    }
}
