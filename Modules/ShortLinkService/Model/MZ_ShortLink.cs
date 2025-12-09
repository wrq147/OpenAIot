using MyAccess.DB.Attr;
using System;

namespace ShortLinkService.Model
{
    public class MZ_ShortLink
    {
        /// <summary>
        /// 短链接映射表
        /// </summary>
        [ID(false)]
        public string Id { get; set; }
        /// <summary>
        /// 映射的url地址
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreatedOn { get; set; }
    }
}
