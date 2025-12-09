using MyAccess.DB.Attr;
using System;

namespace ShortLinkService.Model
{
    [TableName("mz_short_link")]
    public class MZ_ShortLink
    {
        /// <summary>
        /// 短链接映射表
        /// </summary>
        [ID(false)]
        public string Id { get; set; }
        /// <summary>
        /// 所属组织ID
        /// </summary>
        public long? OrgId { get; set; }
        /// <summary>
        /// 映射的url地址
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreatedOn { get; set; }
        /// <summary>
        /// 所属组织名称
        /// </summary>
        [DataIgnore]
        public string OrgName { get; set; }
    }
}
