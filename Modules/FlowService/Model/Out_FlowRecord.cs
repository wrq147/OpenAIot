using System;
using System.Collections.Generic;

namespace FlowService.Model
{
    public class Out_FlowRecord
    {
        public long? Id { get; set; }
        /// <summary>
        /// 完成或取消时间
        /// </summary>
        public DateTime? FinishTime { get; set; }
        /// <summary>
        /// 流程状态
        /// </summary>
        public FlowStatus Status { get; set; }
        public DateTime? create_time { get; set; }
        public DateTime? update_time { get; set; }
        public long createId { get; set; }
        public string RealName { get; set; }
        public long dept_id { get; set; }
        public string dept_name { get; set; }
        public Dictionary<string, string> DataItems { get; set; }
    }
}
