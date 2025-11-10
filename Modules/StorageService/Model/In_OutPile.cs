using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageService.Model
{
    public class In_OutPile
    {
        /// <summary>
        /// 出库单号
        /// </summary>
        public string StockNumber { get; set; }
        /// <summary>
        /// 出库的仓库Id
        /// </summary>
        public string FromHouseId { get; set; }
        /// <summary>
        /// 出库的目标代理商ID
        /// </summary>
        public string ToAgentId { get; set; }
        /// <summary>
        /// 物流单号
        /// </summary>
        public string ExpressNumber { get; set; }
        /// <summary>
        /// 物流公司
        /// </summary>
        public string ExpressCompany { get; set; }
        /// <summary>
        /// 顺风用联系电话
        /// </summary>
        public string ExpressPhone { get; set; }
        /// <summary>
        /// 出库时间
        /// </summary>
        public DateTime OutDate { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 出库物品
        /// </summary>
        public List<In_OutPileItem> Items { get; set; }
    }
}
