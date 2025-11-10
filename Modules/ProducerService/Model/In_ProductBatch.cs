

namespace ProducerService.Model
{
    public class In_ProductBatch
    {
        /// <summary>
        /// 所属产品Id
        /// </summary>
        public string ProductId { get; set; }
        /// <summary>
        /// 生成批号数量
        /// </summary>
        public int Num { get; set; }
    }
}
