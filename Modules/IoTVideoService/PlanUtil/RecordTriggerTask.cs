using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTVideoService.PlanUtil
{

    /// <summary>
    /// 录像计划触发任务（时间轮/任务中心的最小执行单元）
    /// </summary>
    public class RecordTriggerTask
    {
        /// <summary>
        /// 触发时间（按周：最近的该星期对应小时；单日：当天对应小时）
        /// </summary>
        public DateTime TriggerTime { get; set; }

        /// <summary>
        /// 操作类型（Start/End/Both）
        /// </summary>
        public RecordTimeOp OperType { get; set; }

        /// <summary>
        /// 星期几（按周配置：1=周一，7=周日；单日配置：null）
        /// </summary>
        public int? WeekDay { get; set; }

        /// <summary>
        /// 该操作点专属的Cron触发表达式
        /// </summary>
        public string CronExpression { get; set; }
    }

}
