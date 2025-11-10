using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Fields
{
    /// <summary>
    /// 时间
    /// </summary>
    public class TimeField : FieldBase
    {
        /// <summary>
        /// 是否只读
        /// </summary>
        public bool is_readonly { get; set; }
        /// <summary>
        /// 默认值
        /// </summary>
        public double? defval { get; set; }
        /// <summary>
        /// 格式化字符串
        /// </summary>
        public string format { get; set; }
    }
}
