using Common.Attr;
using Common.Share;
using MyAccess.DB.Attr;
using Newtonsoft.Json;
using System;

namespace MonitorService.Model
{
    [TableName("mz_job")]
    public class MZ_Job : BaseEntity
    {
        /// <summary>
        /// 任务序号
        /// </summary>
        [ID(true)]
        public long? job_id { get; set; }
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
        /// 执行表达式,如果为空表示为高频间隔为 0 秒的任务
        /// </summary>
        public string cron_expression { get; set; }
        [DataIgnore]
        [JsonConverter(typeof(OnlySeriaize))]
        public DateTime? nextValidTime { get; set; }
        /// <summary>
        /// 计划策略
        ///  0=默认策略，会根据实际情况自动调整,1=每个失火触发一次,2=所有失火只触发一次执行,3=不追回失火
        /// </summary>
        public string misfire_policy { get; set; }
        /// <summary>
        /// 是否并发执行（0允许 1禁止）
        /// </summary>
        public string concurrent { get; set; }
        /// <summary>
        /// 任务状态（0正常 1暂停）
        /// </summary>
        public string status { get; set; }
    }
}
