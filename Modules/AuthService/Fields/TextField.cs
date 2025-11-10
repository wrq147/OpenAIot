
namespace AuthService.Fields
{
    /// <summary>
    /// 文本
    /// </summary>
    public class TextField : FieldBase
    {
        /// <summary>
        /// 是否只读
        /// </summary>
        public bool is_readonly { get; set; }
        /// <summary>
        /// 是否多行
        /// </summary>
        public bool is_multiple { get; set; }
        /// <summary>
        /// 是否允许扫码输入
        /// </summary>
        public bool is_scan { get; set; }
        /// <summary>
        /// 可修改扫码结果
        /// </summary>
        public bool is_update_scan { get; set; }
        /// <summary>
        /// 引导文字
        /// </summary>
        public string prompt_text { get; set; }
        /// <summary>
        /// 描述文字
        /// </summary>
        public string describe_text { get; set; }


    }
}
