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
                    // 构造Both类型任务（使用0点作为触发时间）
                    DateTime triggerTime = GetNearestWeekTime(week, 0);
                    string cron = GenerateWeekSingleCron(week, 0);

                    tasks.Add(new RecordTriggerTask
                    {
                        TriggerTime = triggerTime,
                        OperType = RecordTimeOp.Both,
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

                // 构造触发任务对象
                DateTime triggerTime = GetNearestWeekTime(range.week, cronHour);
                string cron = GenerateWeekSingleCron(range.week, cronHour);

                tasks.Add(new RecordTriggerTask
                {
                    TriggerTime = triggerTime,
                    OperType = opType,
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

            // Cron格式：秒 分 时 日 月 周 年
            return $"0 0 {cronHour} * * {week} ?";
        }

        /// <summary>
        /// 获取最近的指定星期+小时的触发时间
        /// </summary>
        private static DateTime GetNearestWeekTime(int targetWeek, int targetHour)
        {
            // 处理24点转换为0点
            int actualHour = targetHour == 24 ? 0 : targetHour;

            // 目标星期映射：1=周一，2=周二...7=周日（.NET中DayOfWeek：0=周日，1=周一...6=周六）
            DayOfWeek targetDayOfWeek = targetWeek switch
            {
                1 => DayOfWeek.Monday,
                2 => DayOfWeek.Tuesday,
                3 => DayOfWeek.Wednesday,
                4 => DayOfWeek.Thursday,
                5 => DayOfWeek.Friday,
                6 => DayOfWeek.Saturday,
                7 => DayOfWeek.Sunday,
                _ => DayOfWeek.Monday
            };

            DateTime now = DateTime.Now;
            DateTime triggerTime = new DateTime(now.Year, now.Month, now.Day, actualHour, 0, 0);

            // 计算距离目标星期的天数差
            int daysToAdd = ((int)targetDayOfWeek - (int)now.DayOfWeek + 7) % 7;
            if (daysToAdd == 0 && triggerTime < now)
            {
                // 今天就是目标星期，但时间已过，取下周
                daysToAdd = 7;
            }

            triggerTime = triggerTime.AddDays(daysToAdd);
            return triggerTime;
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
                // 构造Both类型任务（使用0点作为触发时间）
                DateTime triggerTime = GetNearestDayTime(0);
                string cron = GenerateDaySingleCron(0);

                tasks.Add(new RecordTriggerTask
                {
                    TriggerTime = triggerTime,
                    OperType = RecordTimeOp.Both,
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

                // 构造触发任务对象
                DateTime triggerTime = GetNearestDayTime(cronHour);
                string cron = GenerateDaySingleCron(cronHour);

                tasks.Add(new RecordTriggerTask
                {
                    TriggerTime = triggerTime,
                    OperType = opType,
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

            // Cron格式：秒 分 时 日 月 周 年（单日模式周字段用?）
            return $"0 0 {cronHour} * * ? ?";
        }

        /// <summary>
        /// 获取最近的当天/次日指定小时的触发时间
        /// </summary>
        private static DateTime GetNearestDayTime(int targetHour)
        {
            // 处理24点转换为0点
            int actualHour = targetHour == 24 ? 0 : targetHour;

            DateTime now = DateTime.Now;
            DateTime triggerTime = new DateTime(now.Year, now.Month, now.Day, actualHour, 0, 0);

            // 如果当前时间已过目标小时，取次日
            if (triggerTime < now)
            {
                triggerTime = triggerTime.AddDays(1);
            }

            return triggerTime;
        }
        #endregion
    }
}
