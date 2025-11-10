using MyAccess.DB.Attr;
using ProducerService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMService.Model
{
    /// <summary>
    /// 商机明细
    /// </summary>
    [TableName("mz_opport_detail")]
    public class MZ_OpportDetail
    {
        /// <summary>
        /// 商机Id（不用传）
        /// </summary>
        [ID(false)]
        public string OpportId { get; set; }
        /// <summary>
        /// 产品Id
        /// </summary>
        [ID(false)]
        public string ProductId { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public int? Quantity { get; set; }
        /// <summary>
        /// 价格
        /// </summary>
        public decimal? Price { get; set; }
        /// <summary>
        /// 所属企业Id（不用传）
        /// </summary>
        public long? OrgId { get; set; }
        /// <summary>
        /// 排序（不用传）
        /// </summary>
        public int? Sort { get; set; }

        /// <summary>
        /// 删除标志（0代表存在 2代表删除）（不用传）
        /// </summary>
        public string del_flag { get; set; }

        /// <summary>
        /// 产品信息
        /// </summary>
        public MZ_Product ProductInfo { get; set; }
    }
}
