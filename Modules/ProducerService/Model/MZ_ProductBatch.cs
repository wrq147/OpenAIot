using Common.Attr;
using Minio;
using MyAccess.DB.Attr;
using System.Text.Json.Serialization;

namespace ProducerService.Model
{
    /// <summary>
    /// 产品批次表（代表半成品批次、成品设备）
    /// </summary>
    [TableName("mz_product_batch")]
    public class MZ_ProductBatch
    {
        /// <summary>
        /// 编码
        /// </summary>
        [ID(false)]
        public string Id { get; set; }
        /// <summary>
        /// 所属组织ID
        /// </summary>
        public long? OrgId { get; set; }
        /// <summary>
        /// 批次名称
        /// </summary>
        public string BatchName { get; set; }
        /// <summary>
        /// 图片地址
        /// </summary>
        [JsonConverter(typeof(ImageUrl))]
        public string PhotoUrl { get; set; }
        /// <summary>
        /// 批次编号
        /// </summary>
        public string Number { get; set; }
        /// <summary>
        /// 所属产品Id
        /// </summary>
        public string ProductId { get; set; }
    }
}
