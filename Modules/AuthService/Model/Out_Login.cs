
using System;
using System.Text.Json.Serialization;

namespace AuthService
{
    public class Out_Login
    {
        /// <summary>
        /// 令牌
        /// </summary>
        public string token { get; set; }
        /// <summary>
        /// 刷新令牌
        /// </summary>
        public string refresh_token { get; set; }
        /// <summary>
        /// 是否为新创建的
        /// </summary>
        public bool isnew { get; set; } = false;
        /// <summary>
        /// 额外信息
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Out_LoginUserInfo ext_info { get; set; }

    }
}
