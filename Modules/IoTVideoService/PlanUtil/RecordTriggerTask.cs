using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTVideoService.PlanUtil
{
    /// <summary>
    /// 触发操作类型
    /// </summary>
    public enum OperType
    {
        /// <summary>
        /// 启动录像
        /// </summary>
        Start,
        /// <summary>
        /// 停止录像
        /// </summary>
        Stop
    }

    /// <summary>
    /// 录像计划触发任务（时间轮/任务中心的最小执行单元）
    /// </summary>
    public class RecordTriggerTask
    {
        /// <summary>
        /// 触发时间（首次触发时间，循环任务会基于此计算后续时间）
        /// </summary>
        public DateTime TriggerTime { get; set; }

        /// <summary>
        /// 操作类型（启动/停止）
        /// </summary>
        public OperType OperType { get; set; }


        /// <summary>
        /// 循环周期（仅循环任务有效）：Week=每周，Day=每天
        /// </summary>
        public string RecurType { get; set; }

        /// <summary>
        /// 关联的星期数（仅按周循环有效，1=周一，7=周日）
        /// </summary>
        public int? WeekDay { get; set; }

        /// <summary>
        /// 触发时间的时分（HH:mm，用于循环任务计算下次触发时间）
        /// </summary>
        public string TimeOfDay { get; set; }

        /// <summary>
        /// 对应的Cron表达式（适配XXL-Job/Quartz/Hangfire）
        /// 格式：秒 分 时 日 月 周（星期1=周一，7=周日）
        /// </summary>
        public string CronExpression { get; set; }
    }

}
