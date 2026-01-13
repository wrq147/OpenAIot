using System;
using System.Collections.Generic;

namespace IoTService.Models
{
    public class In_HistoryMergeList : In_HistoryMergeBase
    {
        /// <summary>
        /// 设备Id列表
        /// </summary>
        public List<string> Ids { get; set; }
    }
    public class In_HistoryMergeListSync : In_HistoryMergeBase
    {
        /// <summary>
        /// 设备批次编号列表
        /// </summary>
        public List<string> Numbers { get; set; }
    }

    public class In_HistoryMergeBase
    {
        /// <summary>
        /// 最大值:max，最小值：min，平均值：mean，合计：sum，期初值：first，期末值：last，计数：count（传数组）
        /// </summary>
        public List<string> MergeWay { get; set; }
        /// <summary>
        /// 0表示按日，1表示按月，2表示按时，3表示按分，4表示按15分
        /// </summary>
        public int WindowWay { get; set; }
        /// <summary>
        /// 物模型属性标识
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? BeginTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }
        /// <summary>
        /// 过滤时段
        /// </summary>
        public List<TimeHour> Hours { get; set; }
        /// <summary>
        /// 是否启用分组
        /// </summary>
        public bool? IsGroup { get; set; }
    }

    public class TimeHour
    {
        /// <summary>
        /// 开始（小时）
        /// </summary>
        public int StartHour { get; set; }
        /// <summary>
        /// 结束（小时）
        /// </summary>
        public int EndHour { get; set; }
    }
}
