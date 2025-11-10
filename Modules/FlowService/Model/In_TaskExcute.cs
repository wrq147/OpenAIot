using System;
using System.Collections.Generic;
using System.Linq;

namespace FlowService.Model
{
    /// <summary>
    /// 执行流程的参数
    /// </summary>
    public class In_TaskExcute
    {
        /// <summary>
        /// 流程ID
        /// </summary>
        public long flowId { get; set; }
        /// <summary>
        /// 提交的表单数据
        /// </summary>
        public Dictionary<string, object> model { get; set; }
        /// <summary>
        /// 执行的操作key
        /// 通常情况为节点Id
        /// </summary>
        public string key { get; set; }
        /// <summary>
        /// 执行的操作
        /// </summary>
        public string action { get; set; }
    }
}
