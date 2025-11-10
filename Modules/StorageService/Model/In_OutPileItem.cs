using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageService.Model
{
    public class In_OutPileItem
    {
        /// <summary>
        /// 产品批次编号
        /// </summary>
        public string TargetNumber { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public decimal? Quantity { get; set; }
        /// <summary>
        /// 出库价格
        /// </summary>
        public decimal? Price { get; set; }
    }
}
