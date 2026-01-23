using MyAccess.DB.Attr;
using System;
using System.Text.Json.Serialization;

namespace CardService.Model
{
    /// <summary>
    /// 访客消息表
    /// </summary>
    [TableName("mz_card_msg")]
    public class MZ_Card_Msg
    {
        /// <summary>
        /// 编号
        /// </summary>
        [ID(false)]
        public long? Id { get; set; }
        /// <summary>
        /// 访问者
        /// </summary>
        public long? UserId { get; set; }
        /// <summary>
        /// 访问者名片
        /// </summary>
        public long? VisitCardId { get; set; }
        /// <summary>
        /// 来源：0为其它
        /// </summary>
        public int? VisitSource { get; set; }
        /// <summary>
        /// 访问模块,0为名片,1为产品,2为案例
        /// </summary>
        public int? VisitType { get; set; }
        /// <summary>
        /// 记录第几次访问
        /// </summary>
        public int? VisitNumber { get; set; }
        /// <summary>
        /// 目标Id
        /// </summary>
        public long? TargetId { get; set; }
        /// <summary>
        /// 消息接收人
        /// </summary>
        public long? ReceiveUserId { get; set; }
        /// <summary>
        /// 状态（0未读 1已读）
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 访问时间
        /// </summary>
        public DateTime? CreatedOn { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DateTime? EndOn { get; set; }
        /// <summary>
        /// 读取时间
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DateTime? ReadedOn { get; set; }
    }
}
