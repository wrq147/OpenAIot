using Common.Json;
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
        public static RecordTriggerTask GetTodayNextTriggerTask(List<RecordTriggerTask> tasks)
        {
            if (tasks == null || tasks.Count == 0)
                return null;

            try
            {
                DateTime currentTime = DateTime.Now;
                // 标准化基准时间（只保留年月日小时，忽略分秒）
                var normalizedNow = new DateTime(
                    currentTime.Year, currentTime.Month, currentTime.Day,
                    currentTime.Hour, 0, 0);

                RecordTriggerTask nearestTodayTask = null;
                DateTime? nearestTodayTime = null;

                foreach (var task in tasks)
                {
                    // 计算该任务在「当天」的触发时间（仅当天有效）
                    var todayFireTime = GetTaskTodayFireTime(task, normalizedNow);
                    if (!todayFireTime.HasValue)
                        continue;
                    // 只保留晚于当前时间的触发时间
                    if (todayFireTime.Value > normalizedNow)
                    {
                        // 找到当天最小的触发时间
                        if (!nearestTodayTime.HasValue || todayFireTime.Value < nearestTodayTime.Value)
                        {
                            nearestTodayTime = todayFireTime;
                            nearestTodayTask = task;
                        }
                    }
                }
                if (nearestTodayTask != null && nearestTodayTask.OperType == RecordTimeOp.Start)
                {
                    return null;
                }
                return nearestTodayTask;
            }
            catch (Exception)
            {
                return null;
            }
        }
        private static DateTime? GetTaskTodayFireTime(RecordTriggerTask task, DateTime currentTime)
        {
            if (task.Time == 0 && (task.OperType == RecordTimeOp.End || task.OperType == RecordTimeOp.Both))
            {
                return currentTime.Date.AddDays(1);
            }
            // 基础触发小时（已确保0-23）
            int triggerHour = task.Time;
            if (triggerHour < 0 || triggerHour > 23)
                return null;

            // 当天的日期（年-月-日）
            DateTime todayDate = currentTime.Date;

            if (task.WeekDay.HasValue)
            {
                // 按周任务：仅当任务的星期匹配当天星期时，才返回当天触发时间
                int targetWeekDay = task.WeekDay.Value; // 1=周一，7=周日
                int currentWeekDay = currentTime.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)currentTime.DayOfWeek + 1;

                // 星期不匹配 → 当天无触发时间
                if (targetWeekDay != currentWeekDay)
                    return null;

                // 星期匹配 → 返回当天该小时的触发时间
                return todayDate.AddHours(triggerHour);
            }
            else
            {
                // 按日任务：当天必然有触发时间（返回当天该小时）
                return todayDate.AddHours(triggerHour);
            }
        }
        /// <summary>
        /// 将录像配置转换为RecordTriggerTask对象列表
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static List<RecordTriggerTask> GenerateTriggerTasks(MZ_IotRecord data)
        {
            var result = new List<RecordTriggerTask>();
            if (string.IsNullOrEmpty(data.WeekConfig) && string.IsNullOrEmpty(data.TimeConfig))
            {
                return result;
            }

            List<WeekConfigItem> weekTimeRanges = new List<WeekConfigItem>();
            List<TimeConfigItem> dayTimeRanges = new List<TimeConfigItem>();

            // 安全反序列化
            try
            {
                if (!string.IsNullOrEmpty(data.WeekConfig))
                {
                    weekTimeRanges = System.Text.Json.JsonSerializer.Deserialize<List<WeekConfigItem>>(
                        data.WeekConfig, MyDefaultTextJsonConfig.DefaultOptions) ?? new List<WeekConfigItem>();
                }

                if (!string.IsNullOrEmpty(data.TimeConfig))
                {
                    dayTimeRanges = System.Text.Json.JsonSerializer.Deserialize<List<TimeConfigItem>>(
                        data.TimeConfig, MyDefaultTextJsonConfig.DefaultOptions) ?? new List<TimeConfigItem>();
                }
            }
            catch (Exception)
            {
                // 反序列化失败返回空列表
                return result;
            }

            // 参数校验
            if (string.IsNullOrEmpty(data.RecordTimeType))
                return result;

            try
            {
                if (data.RecordTimeType.Equals("week", StringComparison.OrdinalIgnoreCase))
                {
                    result = GenerateWeekTriggerTasks(weekTimeRanges);
                }
                else if (data.RecordTimeType.Equals("time", StringComparison.OrdinalIgnoreCase))
                {
                    result = GenerateDayTriggerTasks(dayTimeRanges);
                }
            }
            catch (Exception)
            {
                // 异常时返回空列表，避免无效数据
            }

            return result;
        }

        #region 按周生成触发任务列表
        /// <summary>
        /// 生成按周的RecordTriggerTask列表（处理0-24范围，合并0(Start)+24(End)为Both）
        /// </summary>
        private static List<RecordTriggerTask> GenerateWeekTriggerTasks(List<WeekConfigItem> weekTimeRanges)
        {
            var tasks = new List<RecordTriggerTask>();

            if (weekTimeRanges == null || weekTimeRanges.Count == 0)
                return tasks;

            // 1. 先按星期分组，便于处理每组内的0-24配对
            var weekGroups = weekTimeRanges.GroupBy(x => x.week).ToList();

            foreach (var group in weekGroups)
            {
                int week = group.Key;
                var items = group.ToList();

                // 过滤无效星期
                if (week < 1 || week > 7)
                    continue;

                // 2. 查找当前星期的0(Start)和24(End)配对
                var start0Item = items.FirstOrDefault(x => x.Time == 0 && x.Op.Equals("Start", StringComparison.OrdinalIgnoreCase));
                var end24Item = items.FirstOrDefault(x => x.Time == 24 && x.Op.Equals("End", StringComparison.OrdinalIgnoreCase));

                // 3. 如果找到配对，生成Both任务并跳过这两个项
                if (start0Item != null && end24Item != null)
                {
                    string cron = GenerateWeekSingleCron(week, 0);

                    tasks.Add(new RecordTriggerTask
                    {
                        OperType = RecordTimeOp.Both,
                        Time = 0,
                        WeekDay = week,
                        CronExpression = cron
                    });

                    // 移除已处理的配对项，只处理剩余项
                    var remainingItems = items.Where(x => !(x.Time == 0 && x.Op.Equals("Start", StringComparison.OrdinalIgnoreCase))
                                                      && !(x.Time == 24 && x.Op.Equals("End", StringComparison.OrdinalIgnoreCase))).ToList();

                    // 处理剩余项
                    ProcessRemainingWeekItems(remainingItems, tasks);
                }
                else
                {
                    // 没有配对，直接处理所有项
                    ProcessRemainingWeekItems(items, tasks);
                }
            }

            return tasks;
        }

        /// <summary>
        /// 处理剩余的非配对周配置项
        /// </summary>
        private static void ProcessRemainingWeekItems(List<WeekConfigItem> items, List<RecordTriggerTask> tasks)
        {
            foreach (var range in items)
            {
                // 过滤无效值（现在允许0-24）
                if (range.week < 1 || range.week > 7 || range.Time < 0 || range.Time > 24)
                    continue;

                // 解析操作类型
                if (!Enum.TryParse(range.Op, true, out RecordTimeOp opType))
                    continue;

                // 特殊处理24点：转换为0点（因为Cron的小时范围是0-23）
                int cronHour = range.Time == 24 ? 0 : range.Time;
                string cron = GenerateWeekSingleCron(range.week, cronHour);

                tasks.Add(new RecordTriggerTask
                {
                    OperType = opType,
                    Time = cronHour,
                    WeekDay = range.week,
                    CronExpression = cron
                });
            }
        }

        /// <summary>
        /// 生成单个按周操作点的Cron表达式
        /// </summary>
        private static string GenerateWeekSingleCron(int week, int hour)
        {
            // 处理24点转换为0点（Cron小时范围0-23）
            int cronHour = hour == 24 ? 0 : hour;

            // Cron格式：秒 分 时 日 月 周
            return $"0 0 {cronHour} * * {week}";
        }


        #endregion

        #region 单日生成触发任务列表
        /// <summary>
        /// 生成单日的RecordTriggerTask列表（处理0-24范围，合并0(Start)+24(End)为Both）
        /// </summary>
        private static List<RecordTriggerTask> GenerateDayTriggerTasks(List<TimeConfigItem> dayTimeRanges)
        {
            var tasks = new List<RecordTriggerTask>();

            if (dayTimeRanges == null || dayTimeRanges.Count == 0)
                return tasks;

            // 1. 查找0(Start)和24(End)配对
            var start0Item = dayTimeRanges.FirstOrDefault(x => x.Time == 0 && x.Op.Equals("Start", StringComparison.OrdinalIgnoreCase));
            var end24Item = dayTimeRanges.FirstOrDefault(x => x.Time == 24 && x.Op.Equals("End", StringComparison.OrdinalIgnoreCase));

            // 2. 如果找到配对，生成Both任务
            if (start0Item != null && end24Item != null)
            {
                string cron = GenerateDaySingleCron(0);
                tasks.Add(new RecordTriggerTask
                {
                    OperType = RecordTimeOp.Both,
                    Time = 0,
                    WeekDay = null,
                    CronExpression = cron
                });

                // 移除已处理的配对项，只处理剩余项
                var remainingItems = dayTimeRanges.Where(x => !(x.Time == 0 && x.Op.Equals("Start", StringComparison.OrdinalIgnoreCase))
                                                          && !(x.Time == 24 && x.Op.Equals("End", StringComparison.OrdinalIgnoreCase))).ToList();

                // 处理剩余项
                ProcessRemainingDayItems(remainingItems, tasks);
            }
            else
            {
                // 没有配对，直接处理所有项
                ProcessRemainingDayItems(dayTimeRanges, tasks);
            }

            return tasks;
        }

        /// <summary>
        /// 处理剩余的非配对单日配置项
        /// </summary>
        private static void ProcessRemainingDayItems(List<TimeConfigItem> items, List<RecordTriggerTask> tasks)
        {
            foreach (var range in items)
            {
                // 过滤无效小时（现在允许0-24）
                if (range.Time < 0 || range.Time > 24)
                    continue;

                // 解析操作类型
                if (!Enum.TryParse(range.Op, true, out RecordTimeOp opType))
                    continue;

                // 特殊处理24点：转换为0点（因为Cron的小时范围是0-23）
                int cronHour = range.Time == 24 ? 0 : range.Time;

                string cron = GenerateDaySingleCron(cronHour);

                tasks.Add(new RecordTriggerTask
                {
                    OperType = opType,
                    Time = cronHour,
                    WeekDay = null,
                    CronExpression = cron
                });
            }
        }

        /// <summary>
        /// 生成单个单日操作点的Cron表达式
        /// </summary>
        private static string GenerateDaySingleCron(int hour)
        {
            // 处理24点转换为0点（Cron小时范围0-23）
            int cronHour = hour == 24 ? 0 : hour;

            // Cron格式：秒 分 时 日 月 周
            return $"0 0 {cronHour} * * ?";
        }
        #endregion
    }
}
