using System;
namespace Common.EventBus
{
    /// <summary>
    /// 消息事件
    /// </summary>
    public class NoticeEvent
    {
        public const string EventKey = "/EV.BUS.NOTICE";
        public NoticeEvent() { }
        public NoticeEvent(long sender, TargetUser[] recv, string[] way)
        {
            this.SendUserId = sender;
            this.RecvUserId = recv;
            this.NoticeWay = way;
        }
        public NoticeEvent(long sender, TargetUser[] recv)
        {
            this.SendUserId = sender;
            this.RecvUserId = recv;
            this.NoticeWay = new string[] { "APP", "EMAIL", "SMS", "WX" };
        }
        /// <summary>
        /// 限制组织
        /// </summary>
        public long OrgId { get; set; }
        /// <summary>
        /// 通知方式:APP站内通知,EMAIL邮件通知,SMS短信通知,WX微信通知
        /// </summary>
        public string[] NoticeWay { get; set; }
        public long SendUserId { get; set; }
        public TargetUser[] RecvUserId { get; set; }
        public string Label { get; set; }
        /// <summary>
        /// 0为普通，1为告警，2为紧急
        /// </summary>
        public int Level { get; set; }
        public string TargetType { get; set; }
        public string TargetUrl { get; set; }
        public string Content { get; set; }
    }
    public class TargetUser
    {
        public long uid { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
    }
}
