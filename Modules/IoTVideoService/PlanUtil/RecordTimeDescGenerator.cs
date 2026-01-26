using Common.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTVideoService.PlanUtil
{
    /// <summary>
    /// 录像计划时段描述生成工具类
    /// </summary>
    public static class RecordTimeDescGenerator
    {
        /// <summary>
        /// 星期名称映射表
        /// </summary>
        private static readonly Dictionary<int, string> _weekNameMap = new Dictionary<int, string>
        {
            { 1, "周一" },
            { 2, "周二" },
            { 3, "周三" },
            { 4, "周四" },
            { 5, "周五" },
            { 6, "周六" },
            { 7, "周日" }
        };

        /// <summary>
        /// 生成录像时段描述文本
        /// </summary>
        /// <param name="recordTimeType">时间类型：week-按周，time-单日</param>
        /// <param name="weekConfig">按周时间配置</param>
        /// <param name="timeConfig">单日时间配置</param>
        /// <returns>格式化的时段描述文本</returns>
        public static string GenerateTimeDesc(string recordTimeType, string weekConfig, string timeConfig)
        {
            if (string.IsNullOrEmpty(weekConfig) && string.IsNullOrEmpty(timeConfig))
            {
                return string.Empty;
            }
            List<WeekConfigItem> weekTimeRanges = System.Text.Json.JsonSerializer.Deserialize<List<WeekConfigItem>>(weekConfig, MyDefaultTextJsonConfig.DefaultOptions);
            List<TimeConfigItem> dayTimeRanges = System.Text.Json.JsonSerializer.Deserialize<List<TimeConfigItem>>(timeConfig, MyDefaultTextJsonConfig.DefaultOptions);
            // 参数校验
            if (string.IsNullOrEmpty(recordTimeType))
                return "未选择时段";

            // 按周配置生成描述
            if (recordTimeType.Equals("week", StringComparison.OrdinalIgnoreCase))
            {
                return GenerateWeekTimeDesc(weekTimeRanges ?? new List<WeekConfigItem>());
            }
            // 单日配置生成描述
            else if (recordTimeType.Equals("time", StringComparison.OrdinalIgnoreCase))
            {
                return GenerateDayTimeDesc(dayTimeRanges ?? new List<TimeConfigItem>());
            }
            // 未知类型
            else
            {
                return "未选择时段";
            }
        }

        /// <summary>
        /// 生成按周的时段描述
        /// </summary>
        private static string GenerateWeekTimeDesc(List<WeekConfigItem> weekTimeRanges)
        {
            if (weekTimeRanges == null || weekTimeRanges.Count == 0)
                return "未选择时段";

            // 按星期分组
            var weekGroups = weekTimeRanges
                .GroupBy(x => x.week)
                .ToDictionary(g => g.Key, g => g.OrderBy(x => x.Time).ToList());

            var descBuilder = new StringBuilder();

            // 遍历每个星期生成描述（按1-7的顺序输出，保证显示顺序合理）
            var orderedWeeks = new List<int> { 1, 2, 3, 4, 5, 6, 7 }; // 关键补充：保证输出顺序是周一到周日
            foreach (var week in orderedWeeks)
            {
                if (!weekGroups.ContainsKey(week))
                    continue;

                var timePoints = weekGroups[week];

                // 获取星期名称
                if (!_weekNameMap.TryGetValue(week, out string weekName))
                    weekName = $"星期{week}";

                // 生成该星期的时间描述
                string weekTimeDesc = GenerateSingleWeekTimeDesc(timePoints);

                if (!string.IsNullOrEmpty(weekTimeDesc))
                {
                    if (descBuilder.Length > 0)
                        descBuilder.Append("；"); // 多星期分隔符

                    descBuilder.Append($"{weekName} {weekTimeDesc}");
                }
            }

            return descBuilder.Length > 0 ? descBuilder.ToString() : "未选择时段";
        }

        /// <summary>
        /// 生成单个星期的时间描述
        /// </summary>
        private static string GenerateSingleWeekTimeDesc(List<WeekConfigItem> timePoints)
        {
            var timeDescList = new List<string>();
            int startHour = -1;

            // 遍历时间点解析时段
            foreach (var point in timePoints)
            {
                RecordTimeOp op;
                if (!Enum.TryParse(point.Op, true, out op))
                    continue;

                switch (op)
                {
                    case RecordTimeOp.Start:
                        // 记录开始时间
                        startHour = point.Time;
                        break;

                    case RecordTimeOp.End:
                        // 结束时间配对生成时段
                        if (startHour >= 0)
                        {
                            timeDescList.Add($"{FormatHour(startHour)}:{FormatMinute(0)}-{FormatHour(point.Time)}:{FormatMinute(0)}");
                            startHour = -1; // 重置开始时间
                        }
                        break;

                    case RecordTimeOp.Both:
                        // 同时开始结束，仅占1小时
                        timeDescList.Add($"{FormatHour(point.Time)}:{FormatMinute(0)}-{FormatHour(point.Time + 1)}:{FormatMinute(0)}");
                        break;
                }
            }

            // 拼接该星期的所有时段
            return string.Join("、", timeDescList);
        }

        /// <summary>
        /// 生成单日的时段描述
        /// </summary>
        private static string GenerateDayTimeDesc(List<TimeConfigItem> dayTimeRanges)
        {
            if (dayTimeRanges == null || dayTimeRanges.Count == 0)
                return "未选择时段";

            var timePoints = dayTimeRanges.OrderBy(x => x.Time).ToList();
            var timeDescList = new List<string>();
            int startHour = -1;

            // 遍历时间点解析时段
            foreach (var point in timePoints)
            {
                RecordTimeOp op;
                if (!Enum.TryParse(point.Op, true, out op))
                    continue;

                switch (op)
                {
                    case RecordTimeOp.Start:
                        // 记录开始时间
                        startHour = point.Time;
                        break;

                    case RecordTimeOp.End:
                        // 结束时间配对生成时段
                        if (startHour >= 0)
                        {
                            timeDescList.Add($"{FormatHour(startHour)}:{FormatMinute(0)}-{FormatHour(point.Time)}:{FormatMinute(0)}");
                            startHour = -1; // 重置开始时间
                        }
                        break;

                    case RecordTimeOp.Both:
                        // 同时开始结束，仅占1小时
                        timeDescList.Add($"{FormatHour(point.Time)}:{FormatMinute(0)}-{FormatHour(point.Time + 1)}:{FormatMinute(0)}");
                        break;
                }
            }

            // 无有效时段
            if (timeDescList.Count == 0)
                return "未选择时段";

            // 拼接单日时段描述
            return $"每日 {string.Join("、", timeDescList)}";
        }

        /// <summary>
        /// 格式化小时（补零）
        /// </summary>
        private static string FormatHour(int hour)
        {
            // 处理24小时的特殊情况
            if (hour >= 24)
                return "24";

            return hour.ToString("D2"); // 补零到2位
        }

        /// <summary>
        /// 格式化分钟（补零）
        /// </summary>
        private static string FormatMinute(int minute)
        {
            return minute.ToString("D2"); // 补零到2位
        }
    }
}
