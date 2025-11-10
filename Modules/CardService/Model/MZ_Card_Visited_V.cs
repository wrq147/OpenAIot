using Common.Attr;
using MyAccess.DB.Attr;
using Newtonsoft.Json;
using System;

namespace CardService.Model
{
    /// <summary>
    /// 受访记录实体
    /// </summary>
    public class MZ_Card_Visited_V : MZ_Card_Msg
    {
        /// <summary>
        /// 访问者名片头像
        /// </summary>
        [DataIgnore]
        [JsonConverter(typeof(ImageUrl), true)]
        public string Avatar { get; set; }
        /// <summary>
        /// 访问者真实姓名
        /// </summary>
        [DataIgnore]
        public string RealName { get; set; }
        /// <summary>
        /// 访问者职位
        /// </summary>
        [DataIgnore]
        public string PostName { get; set; }
        /// <summary>
        /// 访问者组织名
        /// </summary>
        [DataIgnore]
        public string OrgName { get; set; }
        /// <summary>
        /// 访问目标结构
        /// </summary>
        [DataIgnore]
        public object Target { get; set; }
    }
}
