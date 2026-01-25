using System;

namespace MonitorService
{
    public class MonitorOption
    {
        /// <summary>
        /// 是否记录操作日志
        /// </summary>
        public bool log_enable { get; set; }
        /// <summary>
        /// 调度器Id
        /// </summary>
        public string instance_id { get; set; }
    }
}
