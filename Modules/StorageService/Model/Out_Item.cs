using Common.Attr;
using Newtonsoft.Json;

namespace IoTService.Models
{
    /// <summary>
    /// 物品信息
    /// </summary>
    public class Out_Item
    {
        /// <summary>
        /// 存储类型：0半成品、1成品
        /// </summary>
        public int? TargetType { get; set; }
        /// <summary>
        /// 产品批次Id
        /// </summary>
        public string TargetId { get; set; }
        /// <summary>
        /// 目标编号
        /// </summary>
        public string DeviceNumber { get; set; }
        /// <summary>
        /// 目标名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 目标图片
        /// </summary>
        [JsonConverter(typeof(ImageUrl))]
        public string PhotoUrl { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public decimal? Quantity { get; set; }
        /// <summary>
        /// 价格
        /// </summary>
        public decimal? Price { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }
    }
}
