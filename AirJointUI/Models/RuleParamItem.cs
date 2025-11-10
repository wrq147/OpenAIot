using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirJointUI.Models
{
    public class RuleParamItem
    {
        /// <summary>
        /// 参数名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 参数代码
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 参数类型
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 默认值
        /// </summary>
        public object defval { get; set; }
        /// <summary>
        /// 是否只读
        /// </summary>
        public bool readOnly { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string remark { get; set; }
        /// <summary>
        /// 枚举类型使用的枚举元素
        /// </summary>
        public Dictionary<string, string> elements { get; set; }
        /// <summary>
        /// 判断枚举是否多选
        /// </summary>
        public bool multi { get; set; }
    }
}
