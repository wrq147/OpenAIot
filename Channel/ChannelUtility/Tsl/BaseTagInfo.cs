using System;
namespace ChannelUtility.Tsl
{
    /// <summary>
    /// 设备标签信息
    /// </summary>
    public class BaseTagInfo : BaseAll
    {
        /// <summary>
        /// 备注
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 默认值
        /// </summary>
        public object value { get; set; }
        /// <summary>
        /// 绑定的属性标识
        /// </summary>
        public string mapcode { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool enable { get; set; }
        /// <summary>
        /// 类型选项
        /// </summary>
        public BaseValueOption option { get; set; }
    }
}
