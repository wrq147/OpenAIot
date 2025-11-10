using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageService.Model
{
    /// <summary>
    /// 库存表
    /// </summary>
    [TableName("mz_stock_pile")]
    public class MZ_StockPile
    {
        /// <summary>
        /// 所在仓库
        /// </summary>
        [ID(false)]
        public string HouseId { get; set; }
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
        /// 所属企业Id
        /// </summary>
        public long? OrgId { get; set; }
        /// <summary>
        /// 存储数量
        /// </summary>
        public decimal? Quantity { get; set; }
        /// <summary>
        /// 成本均价
        /// </summary>
        public decimal? Price { get; set; }
        /// <summary>
        /// 被锁定数量
        /// </summary>
        public decimal? LockQuantity { get; set; }
        /// <summary>
        /// 库存下限，-1不预警
        /// </summary>
        public int? MinNum { get; set; }
        /// <summary>
        /// 库存上限，-1不预警
        /// </summary>
        public int? MaxNum { get; set; }
        /// <summary>
        /// 是否已发送提醒
        /// </summary>
        public bool? IsTrigger { get; set; }
    }
}
