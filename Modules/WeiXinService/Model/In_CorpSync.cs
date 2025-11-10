using System;

namespace WeiXinService.Model
{
    public class In_CorpSync
    {
        public string appid { get; set; }
        /// <summary>
        /// 速度：1为快速，2为慢速
        /// </summary>
        public int speed { get; set; }
        /// <summary>
        /// 同步的用户名
        /// </summary>
        public string username { get; set; }
    }
}
