using Common.Share;
using System;

namespace MESService.Model
{
    public class In_WorkTaskList : BaseQueryParam
    {
        /// <summary>
        /// 0为进行中，1为已完成
        /// </summary>
        public int? Status { get; set; }
        /// <summary>
        /// 过滤工单
        /// </summary>
        public string WorkOrderId { get; set; }
        /// <summary>
        /// 过滤工序
        /// </summary>
        public string OperId { get; set; }
    }
}
