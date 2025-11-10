using Common.Share;
using System;


namespace MonitorService.Model
{
    public class In_JobList: BaseQueryParam
    {
        /// <summary>
        /// 任务名称
        /// </summary>
        public string jobName { get; set; }
        /// <summary>
        /// 任务组名
        /// </summary>
        public string jobGroup { get; set; }
        /// <summary>
        /// 任务状态
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// 调用目标字符串
        /// </summary>
        public string invoke_target { get; set; }
    }
}
