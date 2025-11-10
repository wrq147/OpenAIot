using Common.Attr;
using Common.Share;
using MyAccess.DB.Attr;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESService.Model
{
    /// <summary>
    /// 物料清单头表
    /// </summary>
    [TableName("mz_bom_header")]
    public class MZ_BomHeader : BaseEntity
    {
        /// <summary>
        /// 编码
        /// </summary>
        [ID(false)]
        public string Id { get; set; }
        /// <summary>
        /// 所属组织ID
        /// </summary>
        [JsonConverter(typeof(OnlySeriaize))]
        public long? OrgId { get; set; }
        /// <summary>
        /// 所属产品Id
        /// </summary>
        public string ProductId { get; set; }
        /// <summary>
        /// 产品名称
        /// </summary>
        [DataIgnore]
        public string ProductName { get; set; }
        /// <summary>
        /// 物料清单明细
        /// </summary>
        [DataIgnore]
        public List<MZ_BomLine> Items { get; set; }
    }
}
