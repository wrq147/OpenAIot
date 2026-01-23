using Common.Attr;
using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StorageService.Model
{
    /// <summary>
    /// 出库单明细实体
    /// </summary>
    [TableName("mz_leave_detail")]
    public class MZ_LeaveDetail
    {
        /// <summary>
        /// 出库单Id
        /// </summary>
        [ID(false)]
        public string StockId { get; set; }
        /// <summary>
        /// 存储类型：0半成品、1成品
        /// </summary>
        [ID(false)]
        public int? TargetType { get; set; }
        /// <summary>
        /// 产品批次Id
        /// </summary>
        [ID(false)]
        public string TargetId { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public decimal? Quantity { get; set; }
        /// <summary>
        /// 出库价格
        /// </summary>
        public decimal? Price { get; set; }
        /// <summary>
        /// 目标编号
        /// </summary>
        [DataIgnore]
        public string TargetNumber { get; set; }
        /// <summary>
        /// 目标名称
        /// </summary>
        [DataIgnore]
        public string TargetName { get; set; }
        /// <summary>
        /// 图片地址
        /// </summary>
        [JsonConverter(typeof(ImageUrl))]
        [DataIgnore]
        public string PhotoUrl { get; set; }
    }
}
