using Common.Share;
using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageService.Model
{
    /// <summary>
    /// 库存记录实体（记录每次出入库后的数量）
    /// </summary>
    [TableName("mz_stock_record")]
    public class MZ_StockRecord
    {
        /// <summary>
        /// 编码
        /// </summary>
        [ID(true)]
        public long? Id { get; set; }
        /// <summary>
        /// 所属企业Id
        /// </summary>
        public long? OrgId { get; set; }
        /// <summary>
        /// 所在仓库
        /// </summary>
        public string HouseId { get; set; }
        /// <summary>
        /// 存储类型：0半成品、1成品
        /// </summary>
        public int? TargetType { get; set; }
        /// <summary>
        /// 耗材或设备Id
        /// </summary>
        public string TargetId { get; set; }
        /// <summary>
        /// 变更前存储数量
        /// </summary>
        public int? Remnant { get; set; }
        /// <summary>
        /// 变更前锁数量
        /// </summary>
        public int? LockRemnant { get; set; }
        /// <summary>
        /// 单据类型：0为出库，1为入库，2为盘亏修正，3为盘盈修正
        /// </summary>
        public int? FormType { get; set; }
        /// <summary>
        /// 单据Id
        /// </summary>
        public string FormId { get; set; }
        /// <summary>
        /// 调整数量
        /// </summary>
        public decimal? Quantity { get; set; }
        /// <summary>
        /// 原库存价格
        /// </summary>
        public decimal? StockPrice { get; set; }
        /// <summary>
        /// 价格
        /// </summary>
        public decimal? Price { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreatedOn { get; set; }
    }
}
