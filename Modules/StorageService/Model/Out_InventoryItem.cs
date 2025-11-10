using Common.Attr;
using MyAccess.DB.Attr;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageService.Model
{
    public class Out_InventoryItem
    {
        /// <summary>
        /// 盘点项Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 唯一编号
        /// </summary>
        public string DeviceNumber { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 图片地址
        /// </summary>
        [JsonConverter(typeof(ImageUrl))]
        public string PhotoUrl { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 存储类型：0半成品、1成品
        /// </summary>
        public int? TargetType { get; set; }
        /// <summary>
        /// 耗材或设备Id
        /// </summary>
        public string TargetId { get; set; }
        /// <summary>
        /// 存储数量
        /// </summary>
        public decimal? Quantity { get; set; }
        /// <summary>
        /// 被锁定数量
        /// </summary>
        public decimal? LockQuantity { get; set; }
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
        /// 差异数量
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
