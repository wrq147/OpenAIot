using Common.Attr;
using Common.Share;
using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESService.Model
{
    /// <summary>
    /// 产品工艺路线
    /// </summary>
    [TableName("mz_product_route")]
    public class MZ_ProductRoute : BaseEntity
    {
        /// <summary>
        /// Id编号
        /// </summary>
        [ID(false)]
        public string Id { get; set; }
        /// <summary>
        /// 所属组织ID
        /// </summary>
        [OnlySeriaize]
        public long? OrgId { get; set; }
        /// <summary>
        /// 工艺路线名称
        /// </summary>
        public string RouteName { get; set; }
        /// <summary>
        /// 产品入库的目标仓库，没有则不自动入库
        /// </summary>
        public string ToHouseId { get; set; }
        /// <summary>
        /// 工艺路线明细
        /// </summary>
        [DataIgnore]
        public List<MZ_ProductRouteOper> Items { get; set; }
    }
}
