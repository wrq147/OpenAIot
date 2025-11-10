using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageService.Model
{
    /// <summary>
    /// 开发者入库接口参数
    /// </summary>
    public class In_Pile
    {
        /// <summary>
        /// 入库单号
        /// </summary>
        public string StockNumber { get; set; }
        /// <summary>
        /// 入库的目标仓库Id
        /// </summary>
        public string ToHouseId { get; set; }
        /// <summary>
        /// 入库时间
        /// </summary>
        public DateTime InDate { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 入库物品
        /// </summary>
        public List<In_PileItem> Items { get; set; }
    }
}
