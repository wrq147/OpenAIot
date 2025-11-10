using Common.Attr;
using MyAccess.DB.Attr;
using Newtonsoft.Json;
using System;

namespace CardService.Model
{
    /// <summary>
    /// 访问过的名片记录实体
    /// </summary>
    public class MZ_Card_Record_V : MZ_Card_Msg
    {
        /// <summary>
        /// 名片头像
        /// </summary>
        [DataIgnore]
        [JsonConverter(typeof(ImageUrl), true)]
        public string Avatar { get; set; }
        /// <summary>
        /// 真实姓名
        /// </summary>
        [DataIgnore]
        public string RealName { get; set; }
        /// <summary>
        /// 职位
        /// </summary>
        [DataIgnore]
        public string PostName { get; set; }
        /// <summary>
        /// 组织名
        /// </summary>
        [DataIgnore]
        public string OrgName { get; set; }
    }
}
