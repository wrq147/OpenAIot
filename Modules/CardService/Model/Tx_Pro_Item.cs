using System;

namespace CardService.Model
{
    /// <summary>
    /// 产品详情元素
    /// </summary>
    public class Tx_Pro_Item
    {
        /// <summary>
        /// 元素类型
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 元素属性
        /// </summary>
        public string attr { get; set; }
        /// <summary>
        /// 元素值
        /// </summary>
        public string data { get; set; }
    }
}
