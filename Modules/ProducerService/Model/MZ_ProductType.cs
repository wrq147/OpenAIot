using Common.Attr;
using MyAccess.DB.Attr;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProducerService.Model
{
    /// <summary>
    /// 产品分组表
    /// </summary>
    [TableName("mz_product_type")]
    public class MZ_ProductType
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
        /// 类型名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 图片地址
        /// </summary>
        [JsonConverter(typeof(ImageUrl))]
        public string PhotoUrl { get; set; }
        /// <summary>
        /// 属性列表，多个用','分隔
        /// </summary>
        public string PropList { get; set; }
        /// <summary>
        /// 排序值：越小越前面
        /// </summary>
        public int? Sort { get; set; }
        /// <summary>
        /// 过滤条件的json
        /// </summary>
        public string ConditionJson { get; set; }
        /// <summary>
        /// 列表字段的json
        /// </summary>
        public string ListFieldsJson { get; set; }
    }
}
