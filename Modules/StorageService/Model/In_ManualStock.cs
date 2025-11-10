using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageService.Model
{
    /// <summary>
    /// 手动入库参数
    /// </summary>
    public class In_ManualStock
    {
        /// <summary>
        /// 编辑的入库单Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 入库单号
        /// </summary>
        public string StockNumber { get; set; }
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
        /// 导入的产品批次
        /// </summary>
        public List<MZ_EnterDetail> List { get; set; }
        /// <summary>
        /// 所入仓库
        /// </summary>
        public string HouseId { get; set; }
        /// <summary>
        /// 入库时间
        /// </summary>
        public DateTime InDate { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
