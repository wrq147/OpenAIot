using Common.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTVideoService
{
    /// <summary>
    /// 录像计划时段描述生成工具类
    /// </summary>
    public static class RecordTimeDescGenerator
    {
        // 星期数转中文映射（数据库week：1=周一，7=周日）
        private static readonly Dictionary<int, string> _weekMap = new Dictionary<int, string>
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
        /// 格式化时间（去掉秒，如 08:00:00 → 08:00）
        /// </summary>
        /// <param name="timeStr">时间字符串</param>
        /// <returns>格式化后的时间</returns>
        private static string FormatTime(string timeStr)
        {
            if (string.IsNullOrEmpty(timeStr) || !timeStr.Contains(":"))
                return timeStr;

            var timeParts = timeStr.Split(':');
            return timeParts.Length >= 2 ? $"{timeParts[0]}:{timeParts[1]}" : timeStr;
        }

        /// <summary>
        /// 合并连续的星期数
        /// </summary>
        /// <param name="weekNums">星期数列表</param>
        /// <returns>合并后的星期描述</returns>
        private static string MergeWeekNumbers(List<int> weekNums)
        {
            if (weekNums == null || weekNums.Count == 0)
                return string.Empty;

            // 去重并排序
            var sortedUniqueWeeks = weekNums.Distinct().OrderBy(w => w).ToList();

            if (sortedUniqueWeeks.Count == 1)
                return _weekMap.TryGetValue(sortedUniqueWeeks[0], out var weekName) ? weekName : string.Empty;

            var result = new List<string>();
            int start = sortedUniqueWeeks[0];
            int prev = sortedUniqueWeeks[0];

            for (int i = 1; i < sortedUniqueWeeks.Count; i++)
            {
                int curr = sortedUniqueWeeks[i];
                // 非连续则拼接上一段
                if (curr - prev > 1)
                {
                    if (start == prev)
                    {
                        result.Add(_weekMap.TryGetValue(start, out var name) ? name : string.Empty);
                    }
                    else
                    {
                        var startName = _weekMap.TryGetValue(start, out var sName) ? sName : string.Empty;
                        var prevName = _weekMap.TryGetValue(prev, out var pName) ? pName : string.Empty;
                        result.Add($"{startName}至{prevName}");
                    }
                    start = curr;
                }
                prev = curr;
            }

            // 处理最后一段
            if (start == prev)
            {
                result.Add(_weekMap.TryGetValue(start, out var name) ? name : string.Empty);
            }
            else
            {
                var startName = _weekMap.TryGetValue(start, out var sName) ? sName : string.Empty;
                var prevName = _weekMap.TryGetValue(prev, out var pName) ? pName : string.Empty;
                result.Add($"{startName}至{prevName}");
            }

            return string.Join("、", result.Where(r => !string.IsNullOrEmpty(r)));
        }

        /// <summary>
        /// 按周配置生成录像时段描述（支持同一天多时段）
        /// </summary>
        /// <param name="weekConfigJson">按周配置JSON字符串</param>
        /// <returns>格式化的时段描述</returns>
        public static string GenerateWeekDesc(string weekConfigJson)
        {
            // 空值处理
            if (string.IsNullOrEmpty(weekConfigJson) || weekConfigJson.Equals("null", StringComparison.OrdinalIgnoreCase) || weekConfigJson == "[]")
                return string.Empty;

            try
            {
                // 解析JSON为对象列表
                var weekConfigs = System.Text.Json.JsonSerializer.Deserialize<List<WeekConfigItem>>(weekConfigJson, MyDefaultTextJsonConfig.DefaultOptions);
                if (weekConfigs == null || weekConfigs.Count == 0)
                    return string.Empty;

                // 步骤1：过滤无效配置并格式化时间
                var validConfigs = weekConfigs
                    .Where(item => _weekMap.ContainsKey(item.Week) && !string.IsNullOrEmpty(item.StartTime) && !string.IsNullOrEmpty(item.EndTime))
                    .Select(item => new WeekConfigItem
                    {
                        Week = item.Week,
                        StartTime = FormatTime(item.StartTime),
                        EndTime = FormatTime(item.EndTime)
                    })
                    .ToList();

                if (validConfigs.Count == 0)
                    return string.Empty;

                // 步骤2：按【时间段】分组，收集每个时间段对应的星期列表
                // 键：startTime_endTime，值：该时间段对应的所有星期数
                var timeToWeeksDict = new Dictionary<string, List<int>>();
                foreach (var config in validConfigs)
                {
                    var timeKey = $"{config.StartTime}_{config.EndTime}";
                    if (!timeToWeeksDict.ContainsKey(timeKey))
                    {
                        timeToWeeksDict[timeKey] = new List<int>();
                    }
                    timeToWeeksDict[timeKey].Add(config.Week);
                }

                // 步骤3：生成每个时间段的描述（支持同一时间段多星期、不同时间段独立展示）
                var timeDescList = new List<string>();
                foreach (var kvp in timeToWeeksDict)
                {
                    var timeParts = kvp.Key.Split('_');
                    var timeDesc = $"{timeParts[0]}-{timeParts[1]}";
                    var weekDesc = MergeWeekNumbers(kvp.Value);

                    if (!string.IsNullOrEmpty(weekDesc))
                    {
                        timeDescList.Add($"{weekDesc} {timeDesc}");
                    }
                }

                // 步骤4：对最终描述按时间排序（可选，提升可读性）
                timeDescList.Sort((a, b) =>
                {
                    // 提取时间段的开始时间进行排序
                    var aTime = a.Split(' ')[1].Split('-')[0];
                    var bTime = b.Split(' ')[1].Split('-')[0];
                    return string.Compare(aTime, bTime, StringComparison.Ordinal);
                });

                return string.Join("、", timeDescList);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"解析按周配置失败：{ex.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// 按时段配置生成录像时段描述
        /// </summary>
        /// <param name="timeConfigJson">按时段配置JSON字符串</param>
        /// <returns>格式化的时段描述</returns>
        public static string GenerateTimeDesc(string timeConfigJson)
        {
            // 空值处理
            if (string.IsNullOrEmpty(timeConfigJson) || timeConfigJson.Equals("null", StringComparison.OrdinalIgnoreCase) || timeConfigJson == "[]")
                return string.Empty;

            try
            {
                // 解析JSON为对象列表
                var timeConfigs = System.Text.Json.JsonSerializer.Deserialize<List<TimeConfigItem>>(timeConfigJson, MyDefaultTextJsonConfig.DefaultOptions);
                if (timeConfigs == null || timeConfigs.Count == 0)
                    return string.Empty;

                // 去重并格式化时间段
                var timeSet = new HashSet<string>();
                var timeDescList = new List<string>();

                foreach (var item in timeConfigs)
                {
                    if (string.IsNullOrEmpty(item.StartTime) || string.IsNullOrEmpty(item.EndTime))
                        continue;

                    var formatStart = FormatTime(item.StartTime);
                    var formatEnd = FormatTime(item.EndTime);
                    var timeKey = $"{formatStart}_{formatEnd}";

                    // 避免重复的时间段
                    if (!timeSet.Contains(timeKey))
                    {
                        timeSet.Add(timeKey);
                        timeDescList.Add($"{formatStart}-{formatEnd}");
                    }
                }

                // 按开始时间排序
                timeDescList.Sort((a, b) =>
                {
                    var timeA = a.Split('-')[0];
                    var timeB = b.Split('-')[0];
                    return string.Compare(timeA, timeB, StringComparison.Ordinal);
                });

                return string.Join("、", timeDescList);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"解析按时段配置失败：{ex.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// 统一生成录像时段描述（兼容按周/按时段，支持同一天多时段）
        /// </summary>
        /// <param name="recordTimeType">时段类型：week/time</param>
        /// <param name="weekConfigJson">按周配置JSON</param>
        /// <param name="timeConfigJson">按时段配置JSON</param>
        /// <returns>最终的时段描述</returns>
        public static string GenerateRecordDesc(string recordTimeType, string weekConfigJson, string timeConfigJson)
        {
            if (string.IsNullOrEmpty(recordTimeType))
                return "未配置时段";

            return recordTimeType.ToLower() switch
            {
                "week" => string.IsNullOrEmpty(GenerateWeekDesc(weekConfigJson)) ? "未配置按周时段" : GenerateWeekDesc(weekConfigJson),
                "time" => string.IsNullOrEmpty(GenerateTimeDesc(timeConfigJson)) ? "未配置按时段时段" : GenerateTimeDesc(timeConfigJson),
                _ => "未配置时段"
            };
        }

        #region 内部模型类
        /// <summary>
        /// 按周配置项模型
        /// </summary>
        private class WeekConfigItem
        {
            public int Week { get; set; }
            public string StartTime { get; set; }
            public string EndTime { get; set; }
        }

        /// <summary>
        /// 按时段配置项模型
        /// </summary>
        private class TimeConfigItem
        {
            public string StartTime { get; set; }
            public string EndTime { get; set; }
        }
        #endregion
    }
}
