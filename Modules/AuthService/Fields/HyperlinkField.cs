using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Fields
{
    /// <summary>
    /// 超链接
    /// </summary>
    public class HyperlinkField : FieldBase
    {
        /// <summary>
        /// 是否只读
        /// </summary>
        public bool is_readonly { get; set; }
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
