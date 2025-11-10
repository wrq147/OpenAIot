using MyAccess.DB.Attr;
using System;

namespace MonitorService.Model
{
    [TableName("mz_job_log")]
    public class MZ_JobLog
    {
        /// <summary>
        /// 任务日志ID
        /// </summary>
        [ID(true)]
        public long? job_log_id { get; set; }
        /// <summary>
        /// 任务名称
        /// </summary>
        public string job_name { get; set; }
        /// <summary>
        /// 任务组名
        /// </summary>
        public string job_group { get; set; }
        /// <summary>
        /// 调用目标字符串
        /// </summary>
        public string invoke_target { get; set; }
        /// <summary>
        /// 日志信息
        /// </summary>
        public string job_message { get; set; }
        /// <summary>
        /// 执行状态（0正常 1失败）
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// 异常信息
        /// </summary>
        public string exception_info { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime create_time { get; set; }
    }
}
