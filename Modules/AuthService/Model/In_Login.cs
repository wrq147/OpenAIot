namespace AuthService
{
    public class In_Login
    {
        public string username { get; set; }
        public string password { get; set; }
        /// <summary>
        /// 验证码
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 验证码唯一标识
        /// </summary>
        public string uuid { get; set; }
    }
}
