using Common.Attr;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AuthService
{
    /// <summary>
    /// 用户返回信息
    /// </summary>
    public class Out_LoginUserInfo
    {
        public long Id { get; set; }
        /// <summary>
        /// 所属组织
        /// </summary>
        public long OrgId { get; set; }
        /// <summary>
        /// 真实姓名
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        [JsonConverter(typeof(ImageUrl), true)]
        public string avatar { get; set; }
        /// <summary>
        /// 用户性别（0男 1女 2未知）
        /// </summary>
        public string Sex { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string mobile { get; set; }
        public string introduction { get; set; }
        /// <summary>
        /// 扩展信息
        /// </summary>
        public Dictionary<string,string> extObj { get; set; }
        /// <summary>
        /// 权限
        /// </summary>
        public HashSet<string> permissions { get; set; }
    }
}
