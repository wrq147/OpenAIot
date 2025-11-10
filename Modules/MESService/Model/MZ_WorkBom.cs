using Common.Attr;
using MyAccess.DB.Attr;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace MESService.Model
{
    /// <summary>
    /// 生产工单物料
    /// </summary>
    [TableName("mz_work_bom")]
    public class MZ_WorkBom
    {
        /// <summary>
        /// Id编号
        /// </summary>
        [ID(false)]
        public string Id { get; set; }
        /// <summary>
        /// 所属组织ID
        /// </summary>
        [JsonConverter(typeof(OnlySeriaize))]
        public long? OrgId { get; set; }
        /// <summary>
        /// 产品Id
        /// </summary>
        public string ProductId { get; set; }
        /// <summary>
        /// 关联的工单Id
        /// </summary>
        public string WorkOrderId { get; set; }
        /// <summary>
        /// 关联的工序Id
        /// </summary>
        public string OperId { get; set; }
        /// <summary>
        /// 所需用量
        /// </summary>
        public decimal? NeedQuantity { get; set; }
        /// <summary>
        /// 实际用量
        /// </summary>
        public decimal? UsedQuantity { get; set; }
        /// <summary>
        /// 单个用量
        /// </summary>
        public decimal? Quantity { get; set; }
    }
}
