using IoTVideoService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTVideoService.PlanUtil
{
    public static class PlanTimeParser
    {

        /// <summary>
        /// 解析录像计划为启停触发任务列表（含Cron表达式）
        /// </summary>
        /// <param name="plan">录像计划实体（需包含核心字段）</param>
        /// <returns>触发任务列表</returns>
        public static List<RecordTriggerTask> Parse(MZ_IotRecord plan)
        {
            if (plan == null)
                throw new ArgumentNullException(nameof(plan));

            var triggerTasks = new List<RecordTriggerTask>();

            // 根据时段类型解析不同配置
            switch (plan.RecordTimeType?.ToLower())
            {
                case "week":
                    triggerTasks.AddRange(ParseWeekConfig(plan.WeekConfig));
                    break;
                case "time":
                    triggerTasks.AddRange(ParseTimeConfig(plan.TimeConfig));
                    break;
                default:
                    throw new NotSupportedException($"不支持的时段类型：{plan.RecordTimeType}");
            }

            
            foreach (var task in triggerTasks)
            {
                // 计算首次触发时间（当前时间之后的第一个触发点）
                task.TriggerTime = GetFirstTriggerTime(task);
                // 生成对应的Cron表达式
                task.CronExpression = GenerateCronExpression(task);
            }

            return triggerTasks;
        }

        /// <summary>
        /// 解析按周配置（WeekConfig）为启停触发任务
        /// </summary>
        /// <param name="weekConfigJson">按周配置JSON字符串</param>
        /// <returns>触发任务列表</returns>
        private static List<RecordTriggerTask> ParseWeekConfig(string weekConfigJson)
        {
            if (string.IsNullOrEmpty(weekConfigJson) || weekConfigJson.Equals("null", StringComparison.OrdinalIgnoreCase) || weekConfigJson == "[]")
                return new List<RecordTriggerTask>();

            try
            {
                // 解析JSON为按周配置项列表
                var weekConfigs = System.Text.Json.JsonSerializer.Deserialize<List<WeekConfigItem>>(weekConfigJson);
                if (weekConfigs == null || weekConfigs.Count == 0)
                    return new List<RecordTriggerTask>();

                var triggerTasks = new List<RecordTriggerTask>();

                foreach (var config in weekConfigs)
                {
                    // 过滤无效配置
                    if (config.Week < 1 || config.Week > 7 || string.IsNullOrEmpty(config.StartTime) || string.IsNullOrEmpty(config.EndTime))
                        continue;

                    // 格式化时间（HH:mm，去掉秒）
                    var startTime = FormatTime(config.StartTime);
                    var endTime = FormatTime(config.EndTime);

                    // 生成启动任务（每周循环）
                    triggerTasks.Add(new RecordTriggerTask
                    {
                        OperType = OperType.Start,
                        RecurType = "Week",
                        WeekDay = config.Week,
                        TimeOfDay = startTime
                    });

                    // 生成停止任务（每周循环）
                    triggerTasks.Add(new RecordTriggerTask
                    {
                        OperType = OperType.Stop,
                        RecurType = "Week",
                        WeekDay = config.Week,
                        TimeOfDay = endTime
                    });
                }

                return triggerTasks;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("解析按周配置JSON失败", ex);
            }
        }

        /// <summary>
        /// 解析按时段配置（TimeConfig）为启停触发任务
        /// </summary>
        /// <param name="timeConfigJson">按时段配置JSON字符串</param>
        /// <returns>触发任务列表</returns>
        private static List<RecordTriggerTask> ParseTimeConfig(string timeConfigJson)
        {
            if (string.IsNullOrEmpty(timeConfigJson) || timeConfigJson.Equals("null", StringComparison.OrdinalIgnoreCase) || timeConfigJson == "[]")
                return new List<RecordTriggerTask>();

            try
            {
                // 解析JSON为按时段配置项列表
                var timeConfigs = System.Text.Json.JsonSerializer.Deserialize<List<TimeConfigItem>>(timeConfigJson);
                if (timeConfigs == null || timeConfigs.Count == 0)
                    return new List<RecordTriggerTask>();

                var triggerTasks = new List<RecordTriggerTask>();

                foreach (var config in timeConfigs)
                {
                    // 过滤无效配置
                    if (string.IsNullOrEmpty(config.StartTime) || string.IsNullOrEmpty(config.EndTime))
                        continue;

                    // 格式化时间（HH:mm，去掉秒）
                    var startTime = FormatTime(config.StartTime);
                    var endTime = FormatTime(config.EndTime);

                    // 生成启动任务（每天循环）
                    triggerTasks.Add(new RecordTriggerTask
                    {
                        OperType = OperType.Start,
                        RecurType = "Day",
                        TimeOfDay = startTime
                    });

                    // 生成停止任务（每天循环）
                    triggerTasks.Add(new RecordTriggerTask
                    {
                        OperType = OperType.Stop,
                        RecurType = "Day",
                        TimeOfDay = endTime
                    });
                }

                return triggerTasks;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("解析按时段配置JSON失败", ex);
            }
        }

        /// <summary>
        /// 生成触发任务对应的Cron表达式（标准6位：秒 分 时 日 月 周）
        /// </summary>
        /// <param name="task">触发任务</param>
        /// <returns>Cron表达式</returns>
        private static string GenerateCronExpression(RecordTriggerTask task)
        {
            if (task == null || string.IsNullOrEmpty(task.TimeOfDay))
                return string.Empty;

            // 解析时分（HH:mm）
            if (!TimeSpan.TryParseExact(task.TimeOfDay, "hh\\:mm", null, out var timeOfDay))
                throw new FormatException($"时间格式错误：{task.TimeOfDay}，请使用HH:mm格式");

            // Cron字段：秒 分 时 日 月 周
            int second = 0; // 固定为0秒触发
            int minute = timeOfDay.Minutes;
            int hour = timeOfDay.Hours;

            switch (task.RecurType)
            {
                case "Week":
                    // 按周循环：周字段指定星期（1=周一，7=周日），日/月为*（任意）
                    if (!task.WeekDay.HasValue || task.WeekDay < 1 || task.WeekDay > 7)
                        throw new InvalidOperationException("按周循环任务必须指定有效星期数");

                    // 适配不同框架的星期值：Quartz/XXL-Job中1=周一，7=周日
                    int weekValue = task.WeekDay.Value;
                    return $"{second} {minute} {hour} * * {weekValue}";

                case "Day":
                    // 每天循环：周字段为?（不指定），日/月为*（任意）
                    return $"{second} {minute} {hour} * * ?";

                default:
                    throw new NotSupportedException($"不支持的循环类型：{task.RecurType}");
            }
        }

        /// <summary>
        /// 计算触发任务的首次触发时间（当前时间之后的第一个有效时间）
        /// </summary>
        /// <param name="task">触发任务</param>
        /// <returns>首次触发时间</returns>
        private static DateTime GetFirstTriggerTime(RecordTriggerTask task)
        {
            var now = DateTime.Now;
            DateTime firstTriggerTime;

            // 解析时分
            if (!TimeSpan.TryParseExact(task.TimeOfDay, "hh\\:mm", null, out var timeOfDay))
                throw new FormatException($"时间格式错误：{task.TimeOfDay}，请使用HH:mm格式");

            // 按循环类型计算首次触发时间
            switch (task.RecurType)
            {
                case "Week":
                    // 按周循环：找到本周/下周的对应星期
                    if (!task.WeekDay.HasValue || task.WeekDay < 1 || task.WeekDay > 7)
                        throw new InvalidOperationException("按周循环任务必须指定有效星期数");

                    // 转换为.NET的DayOfWeek（1=周一→Monday，7=周日→Sunday）
                    var targetDayOfWeek = (DayOfWeek)((task.WeekDay.Value - 1) % 7);
                    var daysToAdd = ((int)targetDayOfWeek - (int)now.DayOfWeek + 7) % 7;

                    // 计算基准日期
                    var baseDate = daysToAdd == 0 ? now.Date : now.Date.AddDays(daysToAdd);
                    firstTriggerTime = baseDate.Add(timeOfDay);

                    // 如果今天就是目标星期，但时间已过，则取下周
                    if (daysToAdd == 0 && firstTriggerTime < now)
                        firstTriggerTime = firstTriggerTime.AddDays(7);
                    break;

                case "Day":
                    // 每天循环：今天/明天的指定时间
                    firstTriggerTime = now.Date.Add(timeOfDay);
                    if (firstTriggerTime < now)
                        firstTriggerTime = firstTriggerTime.AddDays(1);
                    break;

                default:
                    throw new NotSupportedException($"不支持的循环类型：{task.RecurType}");
            }

            return firstTriggerTime;
        }

        /// <summary>
        /// 格式化时间（HH:mm:ss → HH:mm）
        /// </summary>
        /// <param name="timeStr">时间字符串</param>
        /// <returns>格式化后的时间</returns>
        private static string FormatTime(string timeStr)
        {
            if (string.IsNullOrEmpty(timeStr))
                return string.Empty;

            if (timeStr.Contains(":"))
            {
                var parts = timeStr.Split(':');
                return parts.Length >= 2 ? $"{parts[0].PadLeft(2, '0')}:{parts[1].PadLeft(2, '0')}" : timeStr;
            }

            return timeStr;
        }
    }
}
