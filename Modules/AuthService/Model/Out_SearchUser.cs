using System;
namespace AuthService.Model
{
    public class Out_SearchUser
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 姓名、手机号、用户名，按顺序不为空优先显示
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        public string Avatar { get; set; }
    }
}
