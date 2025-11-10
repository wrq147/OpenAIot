using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Fields
{
    /// <summary>
    /// 多选框
    /// </summary>
    public class CheckBoxField : FieldBase
    {
        /// <summary>
        /// 是否允许用户添加选项
        /// </summary>
        public bool is_add { get; set; }
        /// <summary>
        /// 可选项
        /// </summary>
        public string[] optionals { get; set; }
        /// <summary>
        /// 显示方式：下拉、平铺
        /// </summary>
        public string show_way { get; set; }
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
