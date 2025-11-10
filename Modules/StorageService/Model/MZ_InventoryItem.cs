using Common.Attr;
using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageService.Model
{
    /// <summary>
    /// 盘点项实体
    /// </summary>
    [TableName("mz_inventory_item")]
    public class MZ_InventoryItem
    {
        /// <summary>
        /// GUID编码
        /// </summary>
        [ID(false)]
        public string Id { get; set; }
        /// <summary>
        /// 所属企业Id
        /// </summary>
        public long? OrgId { get; set; }
        /// <summary>
        /// 盘点Id
        /// </summary>
        public string InventoryId { get; set; }
        /// <summary>
        /// 所在仓库
        /// </summary>
        public string HouseId { get; set; }
        /// <summary>
        /// 快照数量
        /// </summary>
        public decimal? SnapQuantity { get; set; }
        /// <summary>
        /// 存储类型：0半成品、1成品
        /// </summary>
        public int? TargetType { get; set; }
        /// <summary>
        /// 产品批次Id
        /// </summary>
        public string TargetId { get; set; }
        /// <summary>
        /// 初盘数量
        /// </summary>
        public decimal? FirstCount { get; set; }
        /// <summary>
        /// 复盘数量
        /// </summary>
        public decimal? CheckCount { get; set; }
        /// <summary>
        /// 最终数量
        /// </summary>
        public decimal? Count { get; set; }
        /// <summary>
        /// 最终差异数量
        /// </summary>
        public decimal? DiffCount { get; set; }
        /// <summary>
        /// 初盘人员
        /// </summary>
        public long? FirstUserId { get; set; }
        /// <summary>
        /// 初盘时间
        /// </summary>
        public DateTime? FirstAtTime { get; set; }
        /// <summary>
        /// 复盘人员
        /// </summary>
        public long? CheckUserId { get; set; }
        /// <summary>
        /// 复盘时间
        /// </summary>
        public DateTime? CheckAtTime { get; set; }
    }
}
