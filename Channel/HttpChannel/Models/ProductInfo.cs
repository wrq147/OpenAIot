namespace HttpChannel.Models
{
    public class ProductInfo
    {
        public long ProductId { get; set; }
        public string? ProductName { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string? CreateBy { get; set; }
        /// <summary>
        /// MasterKey
        /// </summary>
        public string? ApiKey { get; set;}
    }
}
