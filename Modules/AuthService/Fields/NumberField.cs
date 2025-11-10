using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Fields
{
    /// <summary>
    /// 数字
    /// </summary>
    public class NumberField : FieldBase
    {
        /// <summary>
        /// 是否只读
        /// </summary>
        public bool is_readonly { get; set; }
        /// <summary>
        /// 是否显示千分位分割符
        /// </summary>
        public bool is_thousandth { get; set; }
        /// <summary>
        /// 小数点位数
        /// </summary>
        public int decimals { get; set; }
        /// <summary>
        /// 默认值
        /// </summary>
        public double? defval { get; set; }
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
