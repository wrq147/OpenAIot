using Common.Share;
using MyAccess.DB.Attr;
using System;

namespace CardService.Model
{
    /// <summary>
    /// 案例实体
    /// </summary>
    [TableName("mz_card_case")]
    public class MZ_Card_Case : BaseEntity
    {
        /// <summary>
        /// 编号
        /// </summary>
        [ID(false)]
        public long? Id { get; set; }
        /// <summary>
        /// 所属组织Id
        /// </summary>
        public long? OrgId { get; set; }
        /// <summary>
        /// 案例标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 案例封面
        /// </summary>
        public string ImageUrl { get; set; }
        /// <summary>
        /// 案例详情
        /// </summary>
        public string Detail { get; set; }
    }
}
