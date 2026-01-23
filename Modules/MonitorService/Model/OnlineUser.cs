using Common.Attr;
using System;
using System.Text.Json.Serialization;
namespace MonitorService.Model
{
    public class OnlineUser
    {
        public long UserId { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        [JsonConverter(typeof(AvatarUrl))]
        public string Avatar { get; set; }
        /// <summary>
        /// ip地址
        /// </summary>
        public string Ipaddr { get; set; }
        /// <summary>
        /// 登录地址
        /// </summary>
        public string Location { get; set; }
        /// <summary>
        /// 客户端浏览器
        /// </summary>
        public string Browser { get; set; }
        /// <summary>
        /// 客户端操作系统
        /// </summary>
        public string OSName { get; set; }
        /// <summary>
        /// 登录时间
        /// </summary>
        public long LoginTime { get; set; }
    }
}
