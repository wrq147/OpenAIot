using MyAccess.DB.Attr;
using System;

namespace CardService.Model
{
    /// <summary>
    /// 通讯录
    /// </summary>
    [TableName("mz_card_holder")]
    public class MZ_CardHolder
    {
        /// <summary>
        /// 关联账号
        /// </summary>
        [ID(false)]
        public long? UserId { get; set; }
        /// <summary>
        /// 目标名片
        /// </summary>
        [ID(false)]
        public long? CardId { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime? CreatedOn { get; set; }
    }
}
