using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelUtility.Tsl
{
    public class BaseOutputValue
    {
        /// <summary>
        /// 名称（必填项）
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 标识符（必填项）
        /// 支持大小写字母、数字和下划线
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 数据类型
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// 枚举类型使用的枚举元素
        /// </summary>
        public Dictionary<string, string> elements { get; set; }
    }
}
