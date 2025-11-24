using AuthService.Fields;
using Common.Share;


namespace ProducerService.Model
{
    public class In_ProductList : BaseQueryParam
    {
        /// <summary>
        /// 搜索的关键词
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 过滤类型
        /// </summary>
        public string TypeId { get; set; }
        /// <summary>
        /// 过滤属性
        /// </summary>
        public string Prop { get; set; }
        /// <summary>
        /// 过滤指定产品Id
        /// </summary>
        public string NoId { get; set; }
        /// <summary>
        /// 是否过滤包含工艺路线的产品
        /// </summary>
        public bool? IsRoute { get; set; }
        /// <summary>
        /// 是否过滤绑定了协议的产品
        /// </summary>
        public bool? IsIot { get; set; }
        /// <summary>
        /// 过滤扩展字段
        /// </summary>
        public FieldFilterItem[] Items { get; set; }
    }
}
