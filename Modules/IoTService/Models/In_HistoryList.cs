using System;
using System.Collections.Generic;

namespace IoTService.Models
{
    public class In_HistoryList : In_HistoryBase
    {
        /// <summary>
        /// 设备的Id
        /// </summary>
        public string Id { get; set; }
    }
    public class In_HistoryListSync : In_HistoryBase
    {
        /// <summary>
        /// 设备的批次编号
        /// </summary>
        public string Number { get; set; }
    }


    public class In_HistoryBase
    {
        /// <summary>
        /// 物模型属性标识（不传则为全部属性的历史数据,当有分页时可以传多个以逗分隔的标识）
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
        /// 分页号
        /// </summary>
        public int? pageNum { get; set; }
        /// <summary>
        /// 分页大小
        /// </summary>
        public int? pageSize { get; set; }
        /// <summary>
        /// 显示总数
        /// </summary>
        public bool? showTotal { get; set; }
    }
}
