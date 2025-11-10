using System;

namespace AuthService
{
    public class AuthConstant
    {
        /// <summary>
        /// Redis Key重复提交用
        /// </summary>
        public const string CONFIG_REPEAT_SUBMIT_KEY = "Rep_";
        /// <summary>
        /// Redis Key配置信息用
        /// </summary>
        public const string CONFIG_CACHE_PRE = "Auth_";
        /// <summary>
        /// 在请求头里的登录令牌Key
        /// </summary>
        public const string CONFIG_TOKEN_KEY = "Authorization";
        /// <summary>
        /// 在请求头里的其它登录令牌Key
        /// </summary>
        public const string CONFIG_OTHER_TOKEN_KEY = "AuthOther";
        /// <summary>
        /// Redis Key区域代码用
        /// </summary>
        public const string CONFIG_AREA_KEY = "Area_Key";
        /// <summary>
        /// Redis Key行业代码用
        /// </summary>
        public const string CONFIG_INDUSTRY_KEY = "Industry_Key";
    }
}
