using Common.Share;
using MyAccess.DB.Attr;
using System;

namespace CardService.Model
{
    /// <summary>
    /// 交换请求实体
    /// </summary>
    [TableName("mz_card_exchange")]
    public class MZ_CardExchange : BaseEntity
    {
        /// <summary>
        /// 编号
        /// </summary>
        [ID(true)]
        public long? Id { get; set; }
        /// <summary>
        /// 请求名片Id
        /// </summary>
        public long? SendCardId { get; set; }
        /// <summary>
        /// 请求人头像
        /// </summary>
        [DataIgnore]
        public string SendAvatar { get; set; }
        /// <summary>
        /// 请求人姓名
        /// </summary>
        [DataIgnore]
        public string SendRealName { get; set; }
        /// <summary>
        /// 请求人职位
        /// </summary>
        [DataIgnore]
        public string SendPostName { get; set; }
        /// <summary>
        /// 请求人组织名称
        /// </summary>
        [DataIgnore]
        public string SendOrgName { get; set; }
        /// <summary>
        /// 接收人名片Id
        /// </summary>
        public long? ReceiveCardId { get; set; }
        /// <summary>
        /// 接收人姓名
        /// </summary>
        [DataIgnore]
        public string RecvRealName { get; set; }
        /// <summary>
        /// 接收人组织名称
        /// </summary>
        [DataIgnore]
        public string RecvOrgName { get; set; }
        /// <summary>
        /// 接收人
        /// </summary>
        public long? ReceiveUserId { get; set; }
        /// <summary>
        /// 状态（0待处理 1同意 2为忽略）
        /// </summary>
        public string Status { get; set; }
    }
}
