using Common.Attr;
using MyAccess.DB.Attr;
using System;
using System.Text.Json.Serialization;
namespace ProducerService.Model
{
    [TableName("mz_iot_product")]
    public class T_IotProduct
    {
        [ID(false)]
        public string Id { get; set; }
        /// <summary>
        /// 所属组织ID
        /// </summary>
        public long? OrgId { get; set; }
        /// <summary>
        /// 产品名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 图片地址
        /// </summary>
        [JsonConverter(typeof(ImageUrl))]
        public string PhotoUrl { get; set; }
        /// <summary>
        /// 备注说明
        /// </summary>
        public string Remark { get; set; }
    }
}
