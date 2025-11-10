using Common.Share;
using System;

namespace CardService.Model
{
    /// <summary>
    /// 受访记录列表请求参数
    /// </summary>
    public class In_Visited_List : BaseQueryParam
    {
        /// <summary>
        /// 消息接收人
        /// </summary>
        public long? ReceiveUserId { get; set; }
        /// <summary>
        /// 访问类型过滤
        /// </summary>
        public int? VisitType { get; set; }
        /// <summary>
        /// 受访目标过滤
        /// </summary>
        public long? TargetId { get; set; }
        /// <summary>
        /// 是否初始化目标
        /// </summary>
        public bool? InitTarget { get; set; }
        /// <summary>
        /// 是否相同访问时只显示最近的一条记录
        /// </summary>
        public bool LastVisited { get; set; }
    }
}
