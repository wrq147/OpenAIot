using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageService.Model
{
    /// <summary>
    /// 库存统计
    /// </summary>
    public class Out_StockStatistics
    {
        /// <summary>
        /// 总数
        /// </summary>
        public int TotalCount { get; set; }
        /// <summary>
        /// 总设备数
        /// </summary>
        public int DevCount { get; set; }
        /// <summary>
        /// 总耗材数
        /// </summary>
        public int PartsCount { get; set; }
        /// <summary>
        /// 今日入库数
        /// </summary>
        public int TodayInCount { get; set; }
        /// <summary>
        /// 今日入库设备数
        /// </summary>
        public int TodayDevInCount { get; set; }
        /// <summary>
        /// 今日入库耗材数
        /// </summary>
        public int TodayPartsInCount { get; set; }
        /// <summary>
        /// 今日出库数
        /// </summary>
        public int TodayOutCount { get; set; }
        /// <summary>
        /// 今日出库成品数
        /// </summary>
        public int TodayDevOutCount { get; set; }
        /// <summary>
        /// 今日出库半成品数
        /// </summary>
        public int TodayPartsOutCount { get; set; }
    }
}
