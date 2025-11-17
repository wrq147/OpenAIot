using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageService.Model
{
    /// <summary>
    /// 开发者入库接口的物品参数
    /// </summary>
    public class In_PileItem
    {
        /// <summary>
        /// 批次编号
        /// </summary>
        public string TargetNumber { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public decimal? Quantity { get; set; }
        /// <summary>
        /// 入库价格
        /// </summary>
        public decimal? Price { get; set; }
    }
}
