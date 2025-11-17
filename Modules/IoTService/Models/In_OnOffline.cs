using System;

namespace IoTService.Models
{
    public class In_OnOffline
    {

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
    }
    public class In_OnOfflineList : In_OnOffline
    {
        /// <summary>
        /// 设备的Id
        /// </summary>
        public string Id { get; set; }
    }
    public class In_OnOfflineListSync : In_OnOffline
    {
        /// <summary>
        /// 设备的批次编号
        /// </summary>
        public string Number { get; set; }
    }
}
