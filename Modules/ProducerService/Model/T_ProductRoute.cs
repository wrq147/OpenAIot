using Common.Attr;
using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProducerService.Model
{
    [TableName("mz_product_route")]
    public class T_ProductRoute
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
    }
}
