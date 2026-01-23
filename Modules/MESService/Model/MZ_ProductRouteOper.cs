using AuthService.Fields;
using Common.Attr;
using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESService.Model
{
    /// <summary>
    /// 产品工艺路线明细
    /// </summary>
    [TableName("mz_product_route_oper")]
    public class MZ_ProductRouteOper : IFieldEntity
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
        /// 工艺路线Id
        /// </summary>
        public string RouteId { get; set; }
        /// <summary>
        /// 工序Id
        /// </summary>
        public string OperId { get; set; }
        /// <summary>
        /// 报工数配比
        /// </summary>
        public decimal? PropOf { get; set; }
        /// <summary>
        /// 工时(分钟)
        /// </summary>
        public decimal? WorkTime { get; set; }
        /// <summary>
        /// 工序顺序
        /// </summary>
        public int? Sequence { get; set; }
        /// <summary>
        /// 扩展的关联对象
        /// </summary>
        [DataIgnore]
        [OnlySeriaize]
        public Dictionary<string, object> ExtObjects { get; set; }
        [DataIgnore]
        public Dictionary<string, object> ExtVals { get; set; }

        public string GetFormId()
        {
            return this.Id;
        }

        public string GetFormName()
        {
            return "工序";
        }
    }
}
